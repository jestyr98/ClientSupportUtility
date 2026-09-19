using System.Diagnostics;
using System.Globalization;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using Microsoft.Win32;

[SupportedOSPlatform("windows")]
internal static class WinSoftwareMenu
{
    public static void Show()
    {
        Menu.Show(
            new MenuOption("OS and Security Patches", ShowOperatingSystem),
            new MenuOption("Binary Installers", ShowInstallers),
            new MenuOption("App Store", ShowAppStore),
            new MenuOption("Back", Action: () => Menu.Show(
                new MenuOption("Software", Show),
                new MenuOption("Hardware", HardwareMenu.Show),
                new MenuOption("Networking", NetworkingMenu.Show),
                new MenuOption("Exit", () => Environment.Exit(0))
            ))
        );
    }

    private static void ShowOperatingSystem()
    {
        Console.Clear();
        Console.WriteLine("Installed OS Update and Security Patches (KB numbers)");
        Console.WriteLine();

        const string subKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Component Based Servicing\Packages";

        var updates = new SortedDictionary<string, DateTime?>(StringComparer.OrdinalIgnoreCase);

        void AddKbNumbers(string text, DateTime? installedDate)
        {
            foreach (Match match in Regex.Matches(text, @"\bKB\d+\b", RegexOptions.IgnoreCase))
            {
                string kbNumber = match.Value.ToUpperInvariant();
                if (!updates.TryGetValue(kbNumber, out DateTime? existingDate)
                    || installedDate > existingDate)
                {
                    updates[kbNumber] = installedDate;
                }
            }
        }

        DateTime? GetInstallDate(RegistryKey packageKey)
        {
            if (packageKey.GetValue("InstallTimeHigh") is not int high
                || packageKey.GetValue("InstallTimeLow") is not int low)
            {
                return null;
            }

            long fileTime = ((long)(uint)high << 32) | (uint)low;
            return fileTime > 0
                ? DateTime.FromFileTimeUtc(fileTime).ToLocalTime()
                : null;
        }

        using RegistryKey? subKey = Registry.LocalMachine.OpenSubKey(subKeyPath);
        if (subKey is null)
        {
            Console.WriteLine("Unable to read the registry. Try running as Administrator.");
        }
        else
        {
            foreach (var name in subKey.GetSubKeyNames())
            {
                using RegistryKey? packageKey = subKey.OpenSubKey(name);
                if (packageKey is null)
                {
                    continue;
                }

                if (packageKey.GetValue("CurrentState") is int currentState && currentState != 112)
                {
                    continue;
                }

                DateTime? installedDate = GetInstallDate(packageKey);
                AddKbNumbers(name, installedDate);

                foreach (var valueName in packageKey.GetValueNames())
                {
                    if (packageKey.GetValue(valueName) is string value)
                    {
                        AddKbNumbers(value, installedDate);
                    }
                }
            }
        }

        if (updates.Count == 0)
        {
            Console.WriteLine("No update packages were found.");
        }
        else
        {
            Console.WriteLine($"{"Date Installed",-15}KB Number");
            Console.WriteLine($"{"-------------",-15}---------");

            foreach (var update in updates)
            {
                string installedDate = update.Value?.ToString("MM/dd/yy") ?? "Unknown";
                Console.WriteLine($"{installedDate,-15}{update.Key}");
            }

            Console.WriteLine();
            Console.WriteLine($"Total: {updates.Count} update(s)");
        }

        Console.WriteLine();
        Console.WriteLine("Press any key to go back...");
        Console.ReadKey(intercept: true);
        Show();
    }

    private static void ShowInstallers()
    {
        Console.Clear();
        Console.WriteLine("Installed MSI Applications");
        Console.WriteLine();

        const string uninstallPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
        var applications = new List<(string Name, string Version, string Publisher, string InstallDate)>();

        string FormatInstallDate(object? value)
        {
            string date = value?.ToString() ?? "";
            return DateTime.TryParseExact(
                date,
                "yyyyMMdd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime parsedDate)
                ? parsedDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)
                : date;
        }

        void ReadUninstallEntries(RegistryHive hive, RegistryView view)
        {
            try
            {
                using RegistryKey baseKey = RegistryKey.OpenBaseKey(hive, view);
                using RegistryKey? uninstallKey = baseKey.OpenSubKey(uninstallPath);
                if (uninstallKey is null)
                {
                    return;
                }

                foreach (string subKeyName in uninstallKey.GetSubKeyNames())
                {
                    using RegistryKey? applicationKey = uninstallKey.OpenSubKey(subKeyName);
                    if (applicationKey is null)
                    {
                        continue;
                    }

                    if (applicationKey.GetValue("WindowsInstaller")?.ToString() != "1")
                    {
                        continue;
                    }

                    string? name = applicationKey.GetValue("DisplayName") as string;
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        continue;
                    }

                    string version = applicationKey.GetValue("DisplayVersion") as string ?? "";
                    string publisher = applicationKey.GetValue("Publisher") as string ?? "";
                    string installDate = FormatInstallDate(applicationKey.GetValue("InstallDate"));

                    applications.Add((name.Trim(), version.Trim(), publisher.Trim(), installDate));
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Some uninstall entries are inaccessible without elevation.
            }
        }

        foreach (RegistryHive hive in new[] { RegistryHive.LocalMachine, RegistryHive.CurrentUser })
        {
            ReadUninstallEntries(hive, RegistryView.Registry64);
            ReadUninstallEntries(hive, RegistryView.Registry32);
        }

        applications = applications
            .OrderBy(application => application.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(application => application.Version, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (applications.Count == 0)
        {
            Console.WriteLine("No installed applications were found.");
        }
        else
        {
            Console.WriteLine($"{"Application",-36}{"Version",-16}{"Publisher",-24}Installed");
            Console.WriteLine($"{"---------",-36}{"-------",-16}{"---------",-24}---------");

            foreach (var application in applications)
            {
                Console.WriteLine($"{TrimForTable(application.Name, 35),-36}"
                    + $"{TrimForTable(application.Version, 15),-16}"
                    + $"{TrimForTable(application.Publisher, 23),-24}"
                    + TrimForTable(application.InstallDate, 11));
            }

            Console.WriteLine();
            Console.WriteLine($"Total: {applications.Count} application(s)");
        }

        Console.WriteLine("Press any key to go back...");
        Console.ReadKey(intercept: true);
        Show();
    }

    private static string TrimForTable(string value, int maxLength)
    {
        return value.Length <= maxLength ? value : value[..(maxLength - 3)] + "...";
    }
    
    private static void ShowAppStore()
    {
        Console.Clear();
        Console.WriteLine("Installed App Store (MSIX/AppX) Packages");
        Console.WriteLine();

        try
        {
            string script = "Get-AppxPackage | Where-Object { $_.SignatureKind -eq 'Store' } | Select-Object Name, Version, Publisher | Sort-Object Name | Format-Table -AutoSize | Out-String -Width 200";

            var startInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -NonInteractive -Command \"{script.Replace("\"", "\\\"")}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo)
                ?? throw new InvalidOperationException("Could not start PowerShell.");

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                Console.WriteLine($"Unable to enumerate App Store packages: {error}");
            }
            else
            {
                Console.WriteLine(output);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unable to enumerate App Store packages: {ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Press any key to go back...");
        Console.ReadKey(intercept: true);
        Show();
    }

    
}
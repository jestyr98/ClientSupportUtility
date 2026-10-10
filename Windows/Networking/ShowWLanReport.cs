using System;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Security.Principal;
using System.Runtime.Versioning;

[SupportedOSPlatform("windows")]
internal static class ShowWLanReport
{
    public static bool RunNetshWlanShowReport()
    {
        try
        {
            Console.WriteLine("Running netsh wlan show wlanreport...");
            if (!IsRunningElevated())
            {
                return RunNetshWlanShowReportElevated();
            }

            var processStartInfo = new ProcessStartInfo
            {
                FileName = "netsh",
                Arguments = "wlan show wlanreport",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            using var process = new Process { StartInfo = processStartInfo };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();
            Console.WriteLine(output);
            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine($"Error: {error}");
            }

            if (process.ExitCode != 0)
            {
                Console.WriteLine($"netsh exited with code {process.ExitCode}; the WLAN report was not generated.");
                return false;
            }

            return true;
        }
        catch (Win32Exception ex) when (ex.NativeErrorCode == 1223)
        {
            const string message = "Elevation was canceled. The WLAN report was not generated.";
            Console.WriteLine(message);
            return false;
        }
        catch (Exception ex)
        {
            var message = $"An error occurred while generating the WLAN report: {ex.Message}";
            Console.WriteLine(message);
            return false;
        }
    }

    private static bool RunNetshWlanShowReportElevated()
    {
        Console.WriteLine("Administrator permission is required. Approve the Windows prompt to continue.");

        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = Path.Combine(Environment.SystemDirectory, "netsh.exe"),
            Arguments = "wlan show wlanreport",
            UseShellExecute = true,
            Verb = "runas",
            WindowStyle = ProcessWindowStyle.Hidden
        });

        if (process is null)
        {
            throw new InvalidOperationException("Could not start elevated netsh.");
        }

        process.WaitForExit();
        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"Elevated netsh exited with code {process.ExitCode}.");
        }

        return true;
    }

    private static bool IsRunningElevated()
    {
        using var identity = WindowsIdentity.GetCurrent();
        return new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
    }

    public static void ShowReportHtml()
    {
        string htmlFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "Microsoft",
            "Windows",
            "WlanReport",
            "wlan-report-latest.html");
        if (!File.Exists(htmlFilePath))
        {
            Console.WriteLine($"File not found: {htmlFilePath}");
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = htmlFilePath,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not open the WLAN report: {ex.Message}");
            return;
        }

        Console.WriteLine("The WLAN report is open. Close it, then press Enter to return to the app.");
        Console.ReadLine();
    }
}
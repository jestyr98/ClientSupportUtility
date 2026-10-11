using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Versioning;
using System.Collections.Generic;



[SupportedOSPlatform("windows")]
internal static class NetworkingMenu
{
    public static void Show()
    {
        Menu.Show(
            new MenuOption("Performance", () => { GetNetworkPerformance(); Show(); }),
            new MenuOption("Diagnostics", () => { GetNetworkDiagnostics(GetAllNetworkAdapters()); Show(); }),
            new MenuOption("Display Active Interfaces", () => { DisplayActiveInterfaces(); WaitForUser(); Show(); }),
            new MenuOption("Open/Closed Ports", () => { GetOpenClosedPorts(); Show(); }),
            new MenuOption("Back", () => Menu.Show(
                new MenuOption("Software", WinSoftwareMenu.Show),
                new MenuOption("Hardware", HardwareMenu.Show),
                new MenuOption("Networking", Show),
                new MenuOption("Exit", () => Environment.Exit(0))
            )),
            new MenuOption("Exit", () => Environment.Exit(0))
        );
    }

    private static void GetNetworkPerformance()
    {
        
    }

    private static void GetNetworkDiagnostics(List<NetworkInterface> adapters)
    {
        var wirelessAdapters = adapters
            .Where(adapter =>
                adapter.OperationalStatus == OperationalStatus.Up &&
                IsWireless(adapter) &&
                !IsNetworkFilterInterface(adapter))
            .ToList();

        if (wirelessAdapters.Count == 0)
        {
            // TODO: Implement additional handling for wired adapters
            Console.WriteLine("Wired adapter diagnostics not implemented yet.");
            return;
        }

        Console.WriteLine("Active wireless adapters:");
        foreach (var adapter in wirelessAdapters)
        {
            Console.WriteLine($"- {adapter.Name}: {adapter.Description}");
        }

        if (ShowWLanReport.RunNetshWlanShowReport())
        {
            ShowWLanReport.ShowReportHtml();
        }
    }

    private static void DisplayActiveInterfaces()
    {
        // Display active interfaces logic here
        var activeAdapters = GetAllNetworkAdapters()
            .Where(adapter => adapter.OperationalStatus == OperationalStatus.Up)
            .ToList();

        Console.WriteLine("Active network interfaces:");
        foreach (var adapter in activeAdapters)
        {
            Console.WriteLine($"- {adapter.Name}: {adapter.Description}");
        }

    }

    private static void WaitForUser()
    {
        Console.WriteLine();
        Console.WriteLine("Press Enter to return to the networking menu...");
        Console.ReadLine();
    }

    private static void GetOpenClosedPorts()
    {
        // Get open/closed ports logic here
        Console.WriteLine("Open/closed ports diagnostics not implemented yet.");
    }

    private static List<NetworkInterface> GetAllNetworkAdapters()
    {
        return NetworkInterface.GetAllNetworkInterfaces().ToList();
    }

    private static bool IsWireless(NetworkInterface adapter)
    {
        return adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211;
    }

    private static bool IsNetworkFilterInterface(NetworkInterface adapter)
    {
        return adapter.Name.Contains("Filter", StringComparison.OrdinalIgnoreCase) ||
               adapter.Description.Contains("Filter", StringComparison.OrdinalIgnoreCase) ||
               adapter.Name.Contains("Packet Scheduler", StringComparison.OrdinalIgnoreCase) ||
               adapter.Description.Contains("Packet Scheduler", StringComparison.OrdinalIgnoreCase) ||
               adapter.Name.Contains("Npcap", StringComparison.OrdinalIgnoreCase) ||
               adapter.Description.Contains("Npcap", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsVPN()
    {
        var activeAdapters = GetAllNetworkAdapters()
            .Where(adapter => adapter.OperationalStatus == OperationalStatus.Up && !IsNetworkFilterInterface(adapter))
            .ToList();

        foreach (var adapter in activeAdapters)
        {
            if (adapter.NetworkInterfaceType == NetworkInterfaceType.Tunnel)
            {
                return true;
            }
        }
        return false;
    }

}
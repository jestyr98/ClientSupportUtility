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
            new MenuOption("Diagnostics", () => { GetNetworkDiagnostics(GetActiveNetworkAdapters()); Show(); }),
            new MenuOption("Display Active Interfaces", () => { DisplayActiveInterfaces(); Show(); }),
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

    private static void GetNetworkDiagnostics(List<NetworkInterface> activeAdapters)
    {
        var wirelessAdapters = activeAdapters
            .Where(adapter => adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
            .ToList();

        if (wirelessAdapters.Count == 0)
        {
            Console.WriteLine("No active wireless adapters found.");
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
    }

    private static void GetOpenClosedPorts()
    {
        // Get open/closed ports logic here
    }

    private static List<NetworkInterface> GetActiveNetworkAdapters()
    {
        var adapters = NetworkInterface.GetAllNetworkInterfaces()
            .Where(ni => ni.OperationalStatus == OperationalStatus.Up)
            .ToList();
        return adapters;
    }

    private static bool IsVPN()
    {
        var adapters = GetActiveNetworkAdapters();
        foreach (var adapter in adapters)
        {
            if (adapter.NetworkInterfaceType == NetworkInterfaceType.Tunnel)
            {
                return true;
            }
        }
        return false;
    }

}
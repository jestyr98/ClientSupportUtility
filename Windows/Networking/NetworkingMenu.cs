using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using Microsoft.Win32;

[SupportedOSPlatform("windows")]
internal static class NetworkingMenu
{
    public static void Show()
    {
        Menu.Show(
            new MenuOption("Performance", () => { GetNetworkPerformance(); }),
            new MenuOption("Diagnostics", () => { GetNetworkDiagnostics(); }),
            new MenuOption("Display Active Interfaces", () => { DisplayActiveInterfaces(); }),
            new MenuOption("Open/Closed Ports", () => { GetOpenClosedPorts(); }),
            new MenuOption("Exit", () => { Environment.Exit(0); })
        );
    }

    private static void GetNetworkPerformance()
    {
        // Get network performance logic here
    }

    private static void GetNetworkDiagnostics()
    {
        // Get network diagnostics logic here
    }

    private static void DisplayActiveInterfaces()
    {
        // Display active interfaces logic here
    }

    private static void GetOpenClosedPorts()
    {
        // Get open/closed ports logic here
    }
}
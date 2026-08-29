

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
        
    }

    private static void ShowInstallers()
    {
        Console.Clear();
        Console.WriteLine("Installers information goes here.");
        Console.WriteLine();
        Console.WriteLine("Press any key to go back...");
        Console.ReadKey(intercept: true);
        Show();
    }
    
    private static void ShowAppStore()
    {
        Console.Clear();
        Console.WriteLine("App Store information goes here.");
        Console.WriteLine();
        Console.WriteLine("Press any key to go back...");
        Console.ReadKey(intercept: true);
        Show();
    }
}
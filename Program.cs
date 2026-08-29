using System.Runtime.InteropServices;
public class Program
{
    public static void Main(string[] args)
    {
        const int bannerWidth = 92;

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(value: new string('*', bannerWidth));
        Console.WriteLine(value: new string('*', bannerWidth));

        WriteBannerLine("  ____   _   _   ____   _____   _____   __  __    ___   _   _   _____   ___");
        WriteBannerLine(" / ___| | | | | / ___| |_   _| | ____| |  \\/  |  |_ _| | \\ | | |  ___| / _ \\");
        WriteBannerLine(" \\___ \\ | |_| | \\___ \\   | |   |  _|   | |\\/| |   | |  |  \\| | | |_   | | | |");
        WriteBannerLine("  ___) | \\__| |  ___) |  | |   | |___  | |  | |   | |  | |\\  | |  _|  | |_| |");
        WriteBannerLine(" |____/  \\___/  |____/   |_|   |_____| |_|  |_|  |___| |_| \\_| |_|     \\___/");
        WriteBannerLine();

        Console.WriteLine(value: new string('*', bannerWidth));
        Console.WriteLine(value: new string('*', bannerWidth));
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(value: "Author: jestyr98☠️");
        Console.ResetColor();
        Console.WriteLine();
        switch (GetOSPlatform())
        {
            case var platform when platform == OSPlatform.Windows:
                Menu.Show(
                    new MenuOption("Software", WinSoftwareMenu.Show),
                    new MenuOption("Hardware", HardwareMenu.Show),
                    new MenuOption("Networking", NetworkingMenu.Show),
                    new MenuOption("Exit", () => Environment.Exit(0))
                );
                break;
            // case var platform when platform == OSPlatform.Linux:
            //     Menu.Show(
            //         new MenuOption("Software", LinuxSoftwareMenu.Show),
            //         new MenuOption("Hardware", HardwareMenu.Show),
            //         new MenuOption("Networking", NetworkingMenu.Show),
            //         new MenuOption("Exit", () => Environment.Exit(0))
            //     );
            //     break;
            // case var platform when platform == OSPlatform.OSX:
            //     Menu.Show(
            //         new MenuOption("Software", MacSoftwareMenu.Show),
            //         new MenuOption("Hardware", HardwareMenu.Show),
            //         new MenuOption("Networking", NetworkingMenu.Show),
            //         new MenuOption("Exit", () => Environment.Exit(0))
            //     );
            //     break;
            default:
                throw new NotSupportedException("Unsupported OS platform.");
        }
    }

    private static void WriteBannerLine(string text = "")
    {
        Console.WriteLine($"****** {text,-78} ******");
    }

    internal static OSPlatform GetOSPlatform()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return OSPlatform.Windows;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return OSPlatform.Linux;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return OSPlatform.OSX;
        throw new NotSupportedException("Unsupported OS platform.");
    }
}

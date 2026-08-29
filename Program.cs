using System;
using System.Runtime.InteropServices;

public class Program
{
    public static void Main(string[] args)
    {
        const int bannerWidth = 92;

        Console.WriteLine(value: new string('*', bannerWidth));
        Console.WriteLine(value: new string('*', bannerWidth));

        WriteBannerLine();
        WriteBannerLine("  ____   _   _   ____   _____   _____   __  __    ___   _   _   _____   ___");
        WriteBannerLine(" / ___| | | | | / ___| |_   _| | ____| |  \\/  |  |_ _| | \\ | | |  ___| / _ \\");
        WriteBannerLine(" \\___ \\ | |_| | \\___ \\   | |   |  _|   | |\\/| |   | |  |  \\| | | |_   | | | |");
        WriteBannerLine("  ___) | \\__| |  ___) |  | |   | |___  | |  | |   | |  | |\\  | |  _|  | |_| |");
        WriteBannerLine(" |____/  \\___/  |____/   |_|   |_____| |_|  |_|  |___| |_| \\_| |_|     \\___/");
        WriteBannerLine();

        Console.WriteLine(value: new string('*', bannerWidth));
        Console.WriteLine(value: new string('*', bannerWidth));
        Console.WriteLine(value: "jestyr98");
        Console.WriteLine();
        Console.WriteLine();
    }

    private static void WriteBannerLine(string text = "")
    {
        Console.WriteLine($"** {text,-86} **");
    }
}

internal static class HardwareMenu
{
    public static void Show()
    {
        Console.Clear();
        // Enter Key emoji from https://emoj.info/enter-key
        Console.WriteLine(value: "Use ⬆️  or ⬇️  to navigate.  Press \u001b[32m⏎\u001b[0m to select an option.");
        Console.WriteLine();
    }
}
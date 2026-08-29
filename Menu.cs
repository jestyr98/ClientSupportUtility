internal sealed record MenuOption(string Label, Action Action);
internal static class Menu
{
    public static void Show(params MenuOption[] options)
    {
        ConsoleKeyInfo key;
        int selection = 1;
        bool isSelected = false;
        bool hasRenderedMenu = false;

        // Enter Key emoji from https://emoj.info/enter-key
        Console.WriteLine("Use ⬆️  or ⬇️  to navigate.  Press \u001b[32m⏎\u001b[0m to select an option.");
        Console.WriteLine();

        while(!isSelected)
        {
            if (hasRenderedMenu)
            {
                Console.Write($"\u001b[{options.Length}A");
            }

            for (int i = 0; i < options.Length; i++)
            {
                WriteMenuItem(options[i].Label, selection == i + 1);
            }
            hasRenderedMenu = true;

            key = Console.ReadKey(intercept: true);

            switch(key.Key)
            {
                case ConsoleKey.DownArrow:
                    selection++;
                    if(selection > options.Length) selection = 1;
                    break;
                case ConsoleKey.UpArrow:
                    selection--;
                    if(selection < 1) selection = options.Length;
                    break;
                case ConsoleKey.Enter:
                    isSelected = true;
                    break;
            }
        }

        options[selection - 1].Action();
    }

    private static void WriteMenuItem(string label, bool isSelected)
    {
        const string eraseLine = "\u001b[2K\r";
        const string selectedPrefix = "✅ \u001b[32m";
        const string resetColor = "\u001b[0m";

        Console.WriteLine(isSelected
            ? $"{eraseLine}{selectedPrefix}{label}{resetColor}"
            : $"{eraseLine}    {label}");
    }
}
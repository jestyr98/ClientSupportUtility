public class Menu
{
    public static void ShowMenu()
    {
        ConsoleKeyInfo key;
        int selection = 1;
        bool isSelected = false;
        bool hasRenderedMenu = false;

        // Enter Key emoji from https://emoj.info/enter-key
        Console.WriteLine("Use ⬆️  or ⬇️  to navigate.  Press \u001b[32m⏎\u001b[0m to select an option.");

        while(!isSelected)
        {
            if (hasRenderedMenu)
            {
                Console.Write("\u001b[4A");
            }

            WriteMenuItem("Software", selection == 1);
            WriteMenuItem("Hardware", selection == 2);
            WriteMenuItem("Networking", selection == 3);
            WriteMenuItem("Exit", selection == 4);
            hasRenderedMenu = true;

            key = Console.ReadKey(intercept: true);

            switch(key.Key)
            {
                case ConsoleKey.DownArrow:
                    selection++;
                    if(selection > 4) selection = 1;
                    break;
                case ConsoleKey.UpArrow:
                    selection--;
                    if(selection < 1) selection = 4;
                    break;
                case ConsoleKey.Enter:
                    isSelected = true;
                    break;
            }
        }

        Console.WriteLine($"You selected option {selection}");
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
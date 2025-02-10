using System.Diagnostics;

namespace neosharp;

class Program
{
    static void Main(string[] args)
    {
        Process p = Process.GetCurrentProcess();    
        
        if (Environment.GetEnvironmentVariable("WT_SESSION") != null || Environment.GetEnvironmentVariable("WT_PROFILE_ID") != null || p.ProcessName == "OpenConsole")
            Console.Clear(); // Clear console if running in Windows Terminal to avoid buffer issues

        Dictionary<string, string> data = new()
        {
            {Computer.GetUsername(), "Name"},
            {new string('-', Computer.GetUsername().Length), "Separator"},
            {"OS", Computer.GetOperatingSystem()},
            {"Build", Computer.GetBuild()},
            {"Installation date", Computer.GetOSInstallDate().ToString()},
            {"Motherboard", Computer.GetMotherboard()},
            {"Uptime", Computer.GetUptime().ToString(@"d\d\ hh\h\ mm\m\ ss\s")},
            {"Resolution", Computer.GetResolution()},
            {"Theme", Computer.GetTheme()},
            {"CPU", Computer.GetCPU()},
            {"Primary GPU", Computer.GetPrimaryGPU()},
            {"Memory", Computer.GetMemory()}
        };

        Console.WriteLine();
        (int, int) before = Console.GetCursorPosition();
        string logoTransformed = LayoutAscii(Environment.OSVersion.Version.Major == 10 ? Computer.LogoWin10 : Computer.LogoWin11);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(logoTransformed);
        (int, int) after = Console.GetCursorPosition();
        PrintData(data, logoTransformed, before);
        Console.SetCursorPosition(after.Item1, after.Item2);
        Console.ResetColor();
        
        if (before.Item2 == 1)
            Console.ReadKey();
    }

    static void PrintData(Dictionary<string, string> data, string layedOutascii, (int, int) pos)
    {
        char[] delims = ['\r', '\n'];
        string[] strings = layedOutascii.Split(delims, StringSplitOptions.RemoveEmptyEntries);
        int longestString = strings.Max(s => s.Length);

        for (int i = 0; i < data.Count; i++)
        {
            KeyValuePair<string, string> pair = data.ElementAt(i);
            Console.SetCursorPosition(longestString, pos.Item2 + i);
            Console.ForegroundColor = ConsoleColor.Yellow;

            if (pair.Value == "Name")
            {
                string[] split = pair.Key.Split('@');
                Console.Write(split[0]);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write('@');
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(split[1]);
                continue;
            }
            else if (pair.Value == "Separator")
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(pair.Key);
                continue;
            }

            Console.Write(pair.Key);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($": {pair.Value}");
        }

        PrintColorbar((longestString, pos.Item2 + data.Count));
    }

    static void PrintColorbar((int, int) pos)
    {

        Console.SetCursorPosition(pos.Item1, pos.Item2 + 2);

        ConsoleColor[] colors = (ConsoleColor[])Enum.GetValues(typeof(ConsoleColor));

        for (int i = 0; i < colors.Length; i++)
        {
            if (i == colors.Length / 2)
                Console.SetCursorPosition(pos.Item1, pos.Item2 + 3);

            ConsoleColor color = colors[i];
            Console.BackgroundColor = color;
            Console.Write(new string(' ', 3));
        }
    }

    static string LayoutAscii(string text)
    {
        int spaces = 6;
        char[] delims = ['\r', '\n'];
        string[] strings = text.Split(delims, StringSplitOptions.RemoveEmptyEntries);
        int longestString = strings.Max(s => s.Length);
        string result = "";

        foreach (string line in strings)
        {
            int spacesAdded = spaces + (longestString - line.Length);
            result += ' ' + line + new string(' ', spacesAdded) + Environment.NewLine;
        }

        return result;
    }
}
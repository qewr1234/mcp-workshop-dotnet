using MyMonkeyApp.Helpers;
using MyMonkeyApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyMonkeyApp;

/// <summary>
/// 콘솔 UI 진입점: 대화형 메뉴를 통해 MonkeyHelper 기능을 호출합니다.
/// </summary>
internal static class Program
{
    private static readonly string[] AsciiArts = new[]
    {
        @"  __  __                 _                  
 |  \/  | ___  _ __ ___ | |__   ___ _ __ ___
 | |\/| |/ _ \\| '_ ` _ \\| '_ \\ / _ \\ '__/ __|
 | |  | | (_) | | | | | | |_) |  __/ |  \\__ \\
 |_|  |_|\\___/|_| |_| |_|_.__/ \\___|_|  |___/",

        @"    .-""-.
   / .===. \
   \/ 6 6 \/
   (  \_/  )
___ooo__V__ooo___",

        @"   .:::.   
  :::::::.  
  :::::::.  
  '::::::::' 
    '::::'    ",

        @"   (\_/)
  ('.')  *  
  (')_(')"
    };

    private static async Task<int> Main()
    {
        try
        {
            await RunMenu().ConfigureAwait(false);
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"오류: {ex.Message}");
            return 1;
        }
    }

    private static async Task RunMenu()
    {
        while (true)
        {
            Console.Clear();
            PrintAsciiArtRandomly();
            PrintHeader();
            Console.WriteLine("1) List all monkeys");
            Console.WriteLine("2) Get details for a specific monkey by name");
            Console.WriteLine("3) Get a random monkey");
            Console.WriteLine("4) Exit app");
            Console.WriteLine();
            Console.Write("Select an option (1-4): ");

            var userInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(userInput))
            {
                continue;
            }

            switch (userInput.Trim())
            {
                case "1":
                    await HandleListAll().ConfigureAwait(false);
                    break;
                case "2":
                    await HandleGetByName().ConfigureAwait(false);
                    break;
                case "3":
                    await HandleRandom().ConfigureAwait(false);
                    break;
                case "4":
                    Console.WriteLine("Goodbye!");
                    return;
                default:
                    Console.WriteLine("Invalid option. Press Enter to continue.");
                    Console.ReadLine();
                    break;
            }
        }
    }

    private static void PrintHeader()
    {
        Console.WriteLine("=== Monkey Console App ===");
        Console.WriteLine();
    }

    private static void PrintAsciiArtRandomly()
    {
        // 40% 확률로 아트 표시
        if (Random.Shared.NextDouble() > 0.6)
        {
            var art = AsciiArts[Random.Shared.Next(AsciiArts.Length)];
            Console.WriteLine(art);
            Console.WriteLine();
        }
    }

    private static async Task HandleListAll()
    {
        var monkeys = await MonkeyHelper.GetMonkeys().ConfigureAwait(false);
        Console.Clear();
        PrintAsciiArtRandomly();
        Console.WriteLine("All available monkeys:\n");
        PrintMonkeyTable(monkeys);
        Console.WriteLine();
        Console.WriteLine("Press Enter to return to menu.");
        Console.ReadLine();
    }

    private static async Task HandleGetByName()
    {
        Console.Write("Enter monkey name: ");
        var userInput = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(userInput))
        {
            Console.WriteLine("Name was empty. Press Enter to continue.");
            Console.ReadLine();
            return;
        }

        var monkey = await MonkeyHelper.GetMonkeyByName(userInput.Trim()).ConfigureAwait(false);
        Console.Clear();
        PrintAsciiArtRandomly();
        if (monkey is null)
        {
            Console.WriteLine($"No monkey found with name '{userInput}'.");
        }
        else
        {
            Console.WriteLine($"Name: {monkey.Name}");
            Console.WriteLine($"Location: {monkey.Location}");
            Console.WriteLine($"Population: {monkey.Population}");
        }

        Console.WriteLine();
        Console.WriteLine("Press Enter to return to menu.");
        Console.ReadLine();
    }

    private static async Task HandleRandom()
    {
        var monkey = await MonkeyHelper.GetRandomMonkey().ConfigureAwait(false);
        Console.Clear();
        PrintAsciiArtRandomly();
        Console.WriteLine("Randomly selected monkey:");
        Console.WriteLine();
        Console.WriteLine($"Name: {monkey.Name}");
        Console.WriteLine($"Location: {monkey.Location}");
        Console.WriteLine($"Population: {monkey.Population}");
        Console.WriteLine();
        Console.WriteLine($"Random pick count: {MonkeyHelper.RandomPickCount}");
        Console.WriteLine();
        Console.WriteLine("Press Enter to return to menu.");
        Console.ReadLine();
    }

    private static void PrintMonkeyTable(List<Monkey> monkeys)
    {
        if (monkeys == null || monkeys.Count == 0)
        {
            Console.WriteLine("No monkeys available.");
            return;
        }

        const int nameWidth = 25;
        const int locWidth = 30;
        const int popWidth = 12;

        // Header
        Console.WriteLine($"{PadRight("Name", nameWidth)} | {PadRight("Location", locWidth)} | {PadRight("Population", popWidth)}");
        Console.WriteLine(new string('-', nameWidth + locWidth + popWidth + 6));

        // Rows
        foreach (var selectedMonkey in monkeys)
        {
            Console.WriteLine($"{PadRight(selectedMonkey.Name, nameWidth)} | {PadRight(selectedMonkey.Location, locWidth)} | {PadRight(selectedMonkey.Population.ToString(), popWidth)}");
        }
    }

    private static string PadRight(string? value, int width) => (value ?? string.Empty).PadRight(width);
}

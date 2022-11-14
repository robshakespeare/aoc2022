using AoC;
using static Crayon.Output;

Console.OutputEncoding = System.Text.Encoding.Unicode;

static void PrintTitle()
{
    Console.Clear();
    Console.WriteLine("🎄 Shakey's AoC 2022 🌟");
}

PrintTitle();

bool exit;
var defaultDay = SolverFactory.DefaultDay;
var cliDays = new Queue<string>(args.Length > 0 ? args : new[] { "" });
do
{
    Console.WriteLine(Green($"Type day number or blank for {defaultDay} or 'x' to exit"));
    var dayNumber = cliDays.TryDequeue(out var cliDay) ? cliDay : Console.ReadLine() ?? "x";
    dayNumber = string.IsNullOrWhiteSpace(dayNumber) ? defaultDay : dayNumber;

    exit = dayNumber is "x" or "exit";
    if (!exit)
    {
        PrintTitle();
        var solver = SolverFactory.Instance.TryCreateSolver(dayNumber);
        if (solver != null)
        {
            await solver.RunAsync();
        }
        else
        {
            Console.WriteLine(Red($"No solver for day '{Bright.Cyan(dayNumber)}'."));
        }
    }
} while (!exit);

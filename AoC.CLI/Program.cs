using System.Text;
using AoC;
using AoC.CLI;
using static Crayon.Output;

Console.OutputEncoding = OperatingSystem.IsWindows() ? Encoding.Unicode : Encoding.UTF8;

switch (args.ElementAtOrDefault(0))
{
    case "--most-recent-day-title":
        var mostRecent = SolverFactory.Instance.Solvers.Last(x => x.DayName != "");
        Console.WriteLine($"Day {mostRecent.DayNumber}: {mostRecent.DayName}");
        return;

    case "--pull":
        await PullPuzzleInputCommand.Instance.DoAsync(args);
        return;

    case "--decrypt":
        await DecryptPuzzleInputsCommand.Instance.DoAsync(args);
        return;
}

static void PrintTitle()
{
    Console.Clear();
    Console.WriteLine($"🎄 Shakey's AoC {Utils.Year} 🌟");
}

PrintTitle();

bool resume;
var defaultDay = SolverFactory.Instance.DefaultDay;
var cliDays = new Queue<string>(args.Length > 0 ? args : new[] { "" });
do
{
    Console.WriteLine(Green($"Type day number, or blank for {defaultDay}, or 'list', or 'x' to exit"));
    var dayNumber = cliDays.TryDequeue(out var cliDay) ? cliDay : Console.ReadLine() ?? "x";
    dayNumber = string.IsNullOrWhiteSpace(dayNumber) ? defaultDay : dayNumber;

    resume = dayNumber is not ("x" or "exit");
    switch (resume)
    {
        case true when dayNumber == "list":
            {
                PrintTitle();
                Console.WriteLine(string.Join(
                    Environment.NewLine,
                    SolverFactory.Instance.Solvers.Where(x => !string.IsNullOrEmpty(x.DayName)).Select(x => $"Day {x.DayNumber}: {x.DayName}")));
                break;
            }
        case true:
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

                break;
            }
    }
} while (resume);

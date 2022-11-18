using AoC;
using static Crayon.Output;

Console.OutputEncoding = System.Text.Encoding.Unicode;

static void PrintTitle()
{
    Console.Clear();
    Console.WriteLine("🎄 Shakey's AoC 2022 🌟");
}

PrintTitle();

var exit = false;
var defaultDay = SolverFactory.Instance.DefaultDay;
var cliDays = new Queue<string>(args.Length > 0 ? args : new[] { "" });
do
{
    Console.WriteLine(Green($"Type day number, or blank for {defaultDay}, or 'list', or 'x' to exit"));
    var dayNumber = cliDays.TryDequeue(out var cliDay) ? cliDay : Console.ReadLine() ?? "x";
    dayNumber = string.IsNullOrWhiteSpace(dayNumber) ? defaultDay : dayNumber;

    switch (dayNumber)
    {
        case "x":
        case "exit":
            exit = true;
            break;

        case "list":
            PrintTitle();
            Console.WriteLine(string.Join(
                Environment.NewLine,
                SolverFactory.Instance.Solvers.Where(x => !string.IsNullOrEmpty(x.DayName)).Select(x => $"{x.DayNumber}: {x.DayName}")));
            break;

        default:
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
} while (!exit);

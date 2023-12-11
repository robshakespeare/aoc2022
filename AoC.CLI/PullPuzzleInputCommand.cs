using System.Text.RegularExpressions;
using Azure.Identity;
using Flurl.Http;
using Microsoft.Extensions.Configuration;
using static Crayon.Output;
using static AoC.CLI.Utils;

namespace AoC.CLI;

internal partial class PullPuzzleInputCommand
{
    private const string UserAgentName = "Rob Shakespeare's AoC CLI https://github.com/robshakespeare";

    public static PullPuzzleInputCommand Instance = new(CryptoInstance);

    private readonly IInputCrypto crypto;

    public PullPuzzleInputCommand(IInputCrypto crypto) => this.crypto = crypto;

    public async Task DoAsync(string[] args)
    {
        Console.Clear();
        try
        {
            var repoRootPath = FindRepoRootPath(AppContext.BaseDirectory);
            var day = args.ElementAtOrDefault(1) ?? SolverFactory.Instance.DefaultDay;
            var year = Year.ToString();
            var keyVaultUri = args.ElementAtOrDefault(2) ?? "https://rws-aoc.vault.azure.net/";

            var dayName = "";

            Task.WaitAll(new[] {
                Task.Run(async () => await PullAndSavePuzzleInputAsync(repoRootPath, day, year, keyVaultUri)),
                Task.Run(async () => dayName = await PullAndSavePuzzleNameAsync(repoRootPath, day, year))
            });

            Console.WriteLine($"Day Name is: {Cyan(dayName)}");

            Console.WriteLine(Bright.Green("✔️ Success"));
        }
        catch (Exception e)
        {
            await Console.Error.WriteLineAsync(Red("Pull command failed: " + e.Message));
            Environment.Exit(1);
        }
    }

    private async Task PullAndSavePuzzleInputAsync(string repoRootPath, string day, string year, string keyVaultUri)
    {
        Console.WriteLine(Bright.Yellow($"Pulling Puzzle Input for Day {Green(day)}"));
        Console.WriteLine(Bright.Black($"Key Vault: {keyVaultUri}{Environment.NewLine}"));

        // Get session token
        var sessionToken = GetSessionToken(keyVaultUri);

        // Get puzzle input
        var puzzleInput = await GetPuzzleInputAsync(day, year, sessionToken);

        // Save puzzle input
        var outputPath = Path.Combine(repoRootPath, "AoC", $"Day{day.PadLeft(2, '0')}", $"input-day{day}.txt");
        await File.WriteAllTextAsync(outputPath, puzzleInput);
        Console.WriteLine($"Puzzle input saved to: {Cyan(outputPath)}");

        // Save puzzle input encrypted
        outputPath = Path.ChangeExtension(outputPath, ".encrypted.txt");
        await File.WriteAllTextAsync(outputPath, crypto.Encrypt(puzzleInput));
    }

    private async Task<string> GetPuzzleInputAsync(string day, string year, string sessionToken)
    {
        using var timing = new TimingBlock("Get Puzzle Input");
        Console.WriteLine(Bright.Black("Getting puzzle input..."));

        var puzzleInput = (await $"https://adventofcode.com/{year}/day/{day}/input"
            .WithCookie("session", sessionToken)
            .WithHeader("User-Agent", UserAgentName)
            .GetStringAsync())
            .ReplaceLineEndings()
            .TrimEnd();

        Console.WriteLine($"Puzzle input retrieved, length: {Green(puzzleInput.Length.ToString())}");
        return puzzleInput;
    }

    [GeneratedRegex(@"--- Day \d+: (?<dayName>.+) ---", RegexOptions.Compiled)]
    private static partial Regex ParseDayNameRegex();

    private static async Task<string> PullAndSavePuzzleNameAsync(string repoRootPath, string day, string year)
    {
        // Get the day name
        var puzzleText = await $"https://adventofcode.com/{year}/day/{day}"
            .WithHeader("User-Agent", UserAgentName)
            .GetStringAsync();

        var dayName = ParseDayNameRegex().Match(puzzleText).Groups["dayName"].Value;

        // Format the name name to escape any characters ready for embedding in to C#
        var dayNameFormatted = Microsoft.CodeAnalysis.CSharp.SymbolDisplay.FormatLiteral(dayName, true);

        // Save puzzle name
        var solverFilePath = Path.Combine(repoRootPath, "AoC", $"Day{day.PadLeft(2, '0')}", $"Day{day}Solver.cs");
        var solverFileContents = await File.ReadAllTextAsync(solverFilePath);
        solverFileContents = solverFileContents.Replace("""public string DayName => "";""", $"""public string DayName => {dayNameFormatted};""");

        await File.WriteAllTextAsync(solverFilePath, solverFileContents);

        return dayName;
    }

    private static string GetSessionToken(string keyVaultUri)
    {
        using var timing = new TimingBlock("Get Session Token");
        Console.WriteLine(Bright.Black("Getting session token..."));

        var sessionToken = new ConfigurationBuilder()
            .AddAzureKeyVault(new Uri(keyVaultUri), new DefaultAzureCredential(new DefaultAzureCredentialOptions
            {
                ExcludeManagedIdentityCredential = true,
                ExcludeVisualStudioCredential = true,
                ExcludeAzurePowerShellCredential = true
            }))
            .Build()["AocSessionToken"];

        if (string.IsNullOrWhiteSpace(sessionToken))
        {
            throw new InvalidOperationException("Session token is empty");
        }

        return sessionToken;
    }
}

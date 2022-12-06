using Azure.Identity;
using Flurl.Http;
using Microsoft.Extensions.Configuration;
using static Crayon.Output;

namespace AoC.CLI;

internal static class PullPuzzleInputCommand
{
    public static async Task DoAsync(string[] args)
    {
        try
        {
            var repoRootPath = FindRepoRootPath(AppContext.BaseDirectory)
                               ?? throw new InvalidOperationException("Could not find repo root");

            var day = args.ElementAtOrDefault(1) ?? SolverFactory.Instance.DefaultDay;
            var year = args.ElementAtOrDefault(2) ?? DateTime.Now.Year.ToString();
            var keyVaultUri = args.ElementAtOrDefault(3) ?? "https://rws-aoc.vault.azure.net/";

            Console.Clear();
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

            Console.WriteLine(Bright.Green("✔️ Success"));
        }
        catch (Exception e)
        {
            await Console.Error.WriteLineAsync(Red("Pull command failed: " + e.Message));
            Environment.Exit(1);
        }
    }

    private static string? FindRepoRootPath(string? dirPath)
    {
        if (dirPath == null)
        {
            return null;
        }

        if (Directory.Exists(Path.Combine(dirPath, ".git")))
        {
            return dirPath;
        }

        var parent = Directory.GetParent(dirPath);
        return parent != null ? FindRepoRootPath(parent.FullName) : null;
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

    private static async Task<string> GetPuzzleInputAsync(string day, string year, string sessionToken)
    {
        using var timing = new TimingBlock("Get Puzzle Input");
        Console.WriteLine(Bright.Black("Getting puzzle input..."));

        var puzzleInput = (await $"https://adventofcode.com/{year}/day/{day}/input"
            .WithCookie("session", sessionToken)
            .WithHeader("User-Agent", "Rob Shakespeare's AoC CLI https://github.com/robshakespeare")
            .GetStringAsync())
            .ReplaceLineEndings("\n");

        Console.WriteLine($"Puzzle input retrieved, length: {Green(puzzleInput.Length.ToString())}");
        return puzzleInput;
    }
}

using static Crayon.Output;
using static AoC.CLI.Utils;
using System.Text.RegularExpressions;

namespace AoC.CLI;

internal partial class DecryptPuzzleInputsCommand(IInputCrypto crypto)
{
    public static DecryptPuzzleInputsCommand Instance = new(CryptoInstance);

    public async Task DoAsync(string[] args)
    {
        Console.Clear();
        using var timing = new TimingBlock("Decrypting puzzle inputs");
        try
        {
            var repoRootPath = FindRepoRootPath(AppContext.BaseDirectory);
            var search = Path.Combine(repoRootPath, "AoC");

            var filePaths = Directory.GetFiles(search, "input*.encrypted.txt", SearchOption.AllDirectories);

            foreach (var filePath in filePaths)
            {
                var outputPath = ChangeExtensionRegex().Replace(filePath, ".txt");
                var puzzleInputCiphered = await File.ReadAllTextAsync(filePath);
                var puzzleInputDeciphered = crypto.Decrypt(puzzleInputCiphered);
                await File.WriteAllTextAsync(outputPath, puzzleInputDeciphered);

                Console.WriteLine($"Decrypted puzzle input saved to: {Cyan(outputPath)}");
            }
        }
        catch (Exception e)
        {
            await Console.Error.WriteLineAsync(Red($"{nameof(DecryptPuzzleInputsCommand)} failed: " + e.Message));
            Environment.Exit(1);
        }
    }

    static readonly Regex Test = ChangeExtensionRegex();

    [GeneratedRegex(@"\.encrypted\.txt$", RegexOptions.Compiled)]
    private static partial Regex ChangeExtensionRegex();
}

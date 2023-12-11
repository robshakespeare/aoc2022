using System.Security.Cryptography;
using static Crayon.Output;

namespace AoC;

internal class InputLoader
{
    private readonly Type _solverType;
    private readonly Lazy<PuzzleInput> _part1;
    private readonly Lazy<PuzzleInput> _part2;

    public PuzzleInput PuzzleInputPart1 => _part1.Value;

    public PuzzleInput PuzzleInputPart2 => _part2.Value;

    internal InputLoader(ISolverBase solver)
    {
        _solverType = solver.GetType();
        var dayNumber = solver.GetDayNumber();
        _part1 = new Lazy<PuzzleInput>(() => LoadInput(GetInputResourceName($"input-day{dayNumber}.txt")));
        _part2 = new Lazy<PuzzleInput>(() =>
        {
            var part2ResourceName = GetInputResourceName($"input-day{dayNumber}-part-2.txt");

            if (_solverType.Assembly.GetManifestResourceInfo(part2ResourceName) == null)
            {
                return _part1.Value;
            }

            Console.WriteLine(Blue("Part 2 has separate input file"));
            return LoadInput(part2ResourceName);
        });
    }

    private string GetInputResourceName(string fileName) => $"{_solverType.Namespace}.{fileName}";

    private string LoadInput(string resourceName)
    {
        using var resourceStream = _solverType.Assembly.GetManifestResourceStream(resourceName);

        if (resourceStream == null)
        {
            Console.WriteLine(Bright.Red($"[WARNING] Input file `{Bright.Cyan(resourceName)}` does not exist"));
            return "";
        }

        using var streamReader = new StreamReader(resourceStream);
        var input = streamReader.ReadToEnd();

        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine(Bright.Red($"[WARNING] Input file `{Bright.Cyan(resourceName)}` is empty"));
        }

        return input.ReplaceLineEndings();
    }
}

public interface IInputCrypto : IDisposable
{
    string Decrypt(string plainText);
    string Encrypt(string cipherText);
}

public sealed class InputCrypto : IInputCrypto
{
    private readonly Aes aes;

    public InputCrypto(byte[] key)
    {
        aes = Aes.Create();
        aes.IV = Encoding.UTF8.GetBytes("@Advent-Of-Code#");
        aes.Key = key;
    }

    public void Dispose() => aes.Dispose();

    public string Encrypt(string plainText)
    {
        using var en = aes.CreateEncryptor();
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, en, CryptoStreamMode.Write);

        using (var sw = new StreamWriter(cs))
        {
            sw.Write(plainText);
        }

        var cipherBytes = ms.ToArray();
        return Convert.ToBase64String(cipherBytes);
    }

    public string Decrypt(string cipherText)
    {
        using var de = aes.CreateDecryptor();
        using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
        using var cs = new CryptoStream(ms, de, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);
        return sr.ReadToEnd();
    }
}

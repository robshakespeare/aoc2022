using System.Text;
using Microsoft.Extensions.Configuration;

namespace AoC.CLI;

internal class Utils
{
    public const int Year = 2022;

    static readonly Lazy<IInputCrypto> cryptoInstance = new(() =>
    {
        var config = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddUserSecrets<Program>()
            .Build();

        const string keyName = "AocPuzzleInputCryptoKey";
        var key = config[keyName];
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new Exception($"Missing config: {keyName}");
        }

        return new InputCrypto(Encoding.UTF8.GetBytes(key));
    });

    public static IInputCrypto CryptoInstance => cryptoInstance.Value;

    public static string FindRepoRootPath(string? dirPath)
    {
        ArgumentNullException.ThrowIfNull(dirPath);

        if (Directory.Exists(Path.Combine(dirPath, ".git")))
        {
            return dirPath;
        }

        var parent = Directory.GetParent(dirPath);
        return parent != null ? FindRepoRootPath(parent.FullName) : throw new InvalidOperationException("Could not find repo root");
    }
}

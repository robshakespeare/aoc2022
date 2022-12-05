using static Crayon.Output;

namespace AoC.CLI;

internal static class PullPuzzleInputCommand
{
    public static void Do()
    {
        try
        {
            var repoRootPath = FindRepoRootPath(Path.GetDirectoryName(typeof(PullPuzzleInputCommand).Assembly.Location))
                               ?? throw new InvalidOperationException("Could not find repo root");

            Console.WriteLine(repoRootPath);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(Red("Pull command failed: " + e.Message));
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
}

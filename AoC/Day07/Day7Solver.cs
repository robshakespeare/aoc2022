namespace AoC.Day07;

public class Day7Solver : ISolver
{
    public string DayName => "No Space Left On Device";

    public long? SolvePart1(PuzzleInput input)
    {
        var rootDir = ParseFilesystem(input);

        return rootDir.ListAllDirectories().Where(dir => dir.TotalSize <= 100000).Sum(dir => dir.TotalSize);
    }

    public long? SolvePart2(PuzzleInput input)
    {
        var rootDir = ParseFilesystem(input);

        const int totalDiskSpaceAvailable = 70000000;
        const int minRequiredUnusedSpace = 30000000;

        var totalAmountOfUsedSpace = rootDir.TotalSize;
        var totalAmountOfUnusedSpace = totalDiskSpaceAvailable - totalAmountOfUsedSpace;
        var minRequiredSizeToDelete = minRequiredUnusedSpace - totalAmountOfUnusedSpace;

        return rootDir.ListAllDirectories()
            .Where(x => x.TotalSize >= minRequiredSizeToDelete)
            .MinBy(x => x.TotalSize)
            ?.TotalSize;
    }

    class ElfDir
    {
        private readonly Dictionary<string, ElfDir> _subDirectories = new();
        private readonly Dictionary<string, long> _files = new();

        string Path { get; }

        public ElfDir? Parent { get; }

        public ElfDir Root { get; }

        public long TotalSize => _files.Values.Sum() + _subDirectories.Values.Sum(subDir => subDir.TotalSize);

        private ElfDir(string path, ElfDir? parent)
        {
            Path = path;
            Parent = parent;
            Root = parent?.Root ?? this;
        }

        public static ElfDir NewRoot() => new("/", null);

        public ElfDir GetSubDirectory(string subDirName) =>
            _subDirectories.TryGetValue(subDirName, out var subDir)
                ? subDir
                : throw new InvalidOperationException($"No sub directory called '{subDirName}' in '{Path}'");

        public ElfDir AddSubDirectory(string subDirName)
        {
            _subDirectories.Add(subDirName, new ElfDir($"{Path}{subDirName}/", this));
            return this;
        }

        public ElfDir AddFile(string fileName, long fileSize)
        {
            _files.Add(fileName, fileSize);
            return this;
        }

        public IEnumerable<ElfDir> ListAllDirectories()
        {
            yield return this;

            foreach (var subSubDir in _subDirectories.Values.SelectMany(subDir => subDir.ListAllDirectories()))
            {
                yield return subSubDir;
            }
        }

        public override string ToString() => Path;
    }

    static ElfDir ParseFilesystem(string input) => input.ReadLines()
        .Select(line => line.Split(" "))
        .Aggregate(ElfDir.NewRoot(), (currentDirectory, parts) => parts switch
        {
            ["$", "cd", "/"] => currentDirectory.Root,
            ["$", "cd", ".."] => currentDirectory.Parent ??
                                 throw new InvalidOperationException("Cannot move out one level from root"),
            ["$", "cd", var dirName] => currentDirectory.GetSubDirectory(dirName),
            ["$", "ls"] => currentDirectory,
            ["dir", var dirName] => currentDirectory.AddSubDirectory(dirName),
            [var fileSize, var fileName] => currentDirectory.AddFile(fileName, long.Parse(fileSize)),
            _ => throw new InvalidOperationException("Unexpected line: " + string.Join(" ", parts))
        }).Root;
}

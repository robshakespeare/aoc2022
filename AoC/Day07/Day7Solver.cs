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

        public ElfDir GetDirectory(string subDirName) =>
            _subDirectories.TryGetValue(subDirName, out var subDir)
                ? subDir
                : throw new InvalidOperationException($"No sub directory called '{subDirName}' in '{Path}'");

        public void AddSubDirectory(string subDirName) => _subDirectories.Add(subDirName, new ElfDir($"{Path}{subDirName}/", this));

        public void AddFile(string fileName, long fileSize) => _files.Add(fileName, fileSize);

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

    static ElfDir ParseFilesystem(string input)
    {
        var currentDirectory = ElfDir.NewRoot();
        var inputLines = new Queue<string>(input.ReadLines());

        while (inputLines.Count > 0)
        {
            var line = inputLines.Dequeue();
            var parts = line.Split(" ");

            if (parts[0] == "$")
            {
                var command = parts[1];

                switch (command)
                {
                    case "cd":
                        currentDirectory = ChangeDirectoryCommand(parts[2], currentDirectory);
                        break;
                    case "ls":
                        ListCommand(inputLines, currentDirectory);
                        break;
                    default:
                        throw new InvalidOperationException("Unexpected command: " + command);
                }
            }
            else
            {
                throw new InvalidOperationException("Unexpected line: " + line);
            }
        }

        return currentDirectory.Root;
    }

    private static ElfDir ChangeDirectoryCommand(string arg, ElfDir currentDirectory)
    {
        switch (arg)
        {
            case "/":
                return currentDirectory.Root;
            case "..":
                return currentDirectory.Parent ??
                       throw new InvalidOperationException("Cannot move out one level from root");
            default:
                if (string.IsNullOrEmpty(arg))
                {
                    throw new InvalidOperationException("Empty change directory argument");
                }

                return currentDirectory.GetDirectory(arg);
        }
    }

    static void ListCommand(Queue<string> inputLines, ElfDir currentDirectory)
    {
        while (inputLines.Count > 0 && !inputLines.Peek().StartsWith("$"))
        {
            var contents = inputLines.Dequeue().Split(" ");

            if (contents[0] == "dir")
            {
                currentDirectory.AddSubDirectory(contents[1]);
            }
            else
            {
                var fileSize = long.Parse(contents[0]);
                var fileName = contents[1];

                currentDirectory.AddFile(fileName, fileSize);
            }
        }
    }
}

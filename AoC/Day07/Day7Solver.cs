namespace AoC.Day07;

public class Day7Solver : ISolver
{
    public string DayName => "No Space Left On Device";

    public long? SolvePart1(PuzzleInput input)
    {
        var rootDir = ParseFilesystem(input);

        return rootDir.ListAll().Where(dir => dir.TotalSize <= 100000).Sum(dir => dir.TotalSize);
    }

    public long? SolvePart2(PuzzleInput input)
    {
        var rootDir = ParseFilesystem(input);

        const int totalDiskSpaceAvailable = 70000000;
        const int minRequiredUnusedSpace = 30000000;

        var totalAmountOfUsedSpace = rootDir.TotalSize;
        var totalAmountOfUnusedSpace = totalDiskSpaceAvailable - totalAmountOfUsedSpace;
        var minRequiredSizeToDelete = minRequiredUnusedSpace - totalAmountOfUnusedSpace;

        return rootDir.ListAll()
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

        public ElfDir GetDirectory(string subDirectoryName) =>
            _subDirectories.TryGetValue(subDirectoryName, out var subDirectory)
                ? subDirectory
                : throw new InvalidOperationException($"No sub directory called '{subDirectoryName}' in '{Path}'");

        public void AddSubDirectory(string subDirectoryName) => _subDirectories.Add(subDirectoryName, new ElfDir($"{Path}{subDirectoryName}/", this));

        public void AddFile(string fileName, long fileSize) => _files.Add(fileName, fileSize);

        public IEnumerable<ElfDir> ListAll()
        {
            yield return this;

            foreach (var subSubDir in _subDirectories.Values.SelectMany(subDir => subDir.ListAll()))
            {
                yield return subSubDir;
            }
        }

        public override string ToString() => Path;
    }

    static ElfDir ParseFilesystem(string input)
    {
        var rootDirectory = ElfDir.NewRoot();
        var currentDirectory = rootDirectory;
        var inputLines = new Stack<string>(input.ReadLines().Reverse());

        while (inputLines.Count > 0)
        {
            var line = inputLines.Pop();
            var parts = line.Split(" ");

            if (parts[0] == "$")
            {
                var command = parts[1];

                switch (command)
                {
                    case "cd":
                    {
                        currentDirectory = ChangeDirectoryCommand(parts[2], currentDirectory);
                        break;
                    }
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

        return rootDirectory;
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

    static void ListCommand(Stack<string> inputLines, ElfDir currentDirectory)
    {
        while (inputLines.Count > 0 && !inputLines.Peek().StartsWith("$"))
        {
            var contents = inputLines.Pop().Split(" ");

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

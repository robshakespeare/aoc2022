using System.Runtime.CompilerServices;

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
        return null;
    }

    public class ElfDir
    {
        private readonly Dictionary<string, ElfDir> _subDirectories = new();
        private readonly Dictionary<string, long> _files = new();

        public string Path { get; }

        public ElfDir? Parent { get; }

        public long TotalSize => _files.Values.Sum() + _subDirectories.Values.Sum(subDir => subDir.TotalSize);

        public ElfDir(string path, ElfDir? parent)
        {
            Path = path;
            Parent = parent;
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
        //var currentDirectoryPath = new Stack<string>(new[] { "/" });

        var rootDirectory = ElfDir.NewRoot();
        var currentDirectory = rootDirectory;

        var inputLines = new Stack<string>(input.ReadLines().Reverse());

        //using var lineReader = input.ReadLines().GetEnumerator();

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
                        var arg = parts[2];

                        switch (arg)
                        {
                            case "/":
                                //currentDirectoryPath = new Stack<string>(new[] {"/"});
                                currentDirectory = rootDirectory;
                                break;
                            case "..":
                                currentDirectory = currentDirectory.Parent ?? throw new InvalidOperationException("Cannot move out one level from root");
                                break;
                            default:
                                if (string.IsNullOrEmpty(arg))
                                {
                                    throw new InvalidOperationException("Unexpected change directory argument: " + command);
                                }

                                currentDirectory = currentDirectory.GetDirectory(arg);
                                //currentDirectoryPath.Push(arg);
                                break;
                        }

                        break;
                    }
                    case "ls":
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
}

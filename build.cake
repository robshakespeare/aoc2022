#addin nuget:?package=Cake.FileHelpers&version=5.0.0

var target = Argument<string>("target", "CreateAllDays");
var day = Argument<string>("day", "");

Task("CreateDay")
    .Does(() =>
    {
        if (string.IsNullOrWhiteSpace(day))
        {
            Error($"Usage: dotnet cake --day=X");
            return;
        }

        var dayPadded = day.PadLeft(2, '0');

        Information($"Ensuring files for day {day} exist...");

        const string workingDir = "./.cake-working/";
        CreateDirectory(workingDir);
        CleanDirectory(workingDir);

        CopyFiles("./Template/**/*.*", workingDir, true);
        ReplaceTextInFiles("./.cake-working/**/*.*", "NNN", day);
        ReplaceTextInFiles("./.cake-working/**/*.*", "XXX", dayPadded);

        foreach(var file in GetFiles("./.cake-working/**/*.*"))
        {
            var newFilePath = file.GetDirectory().CombineWithFilePath(file.GetFilename().FullPath.Replace("NNN", day));
            MoveFile(file, newFilePath);
        }

        foreach(var dir in GetDirectories("./.cake-working/*/*"))
        {
            var newDirPath = dir.FullPath.Replace("DayXXX", $"Day{dayPadded}");
            MoveDirectory(dir, newDirPath);
        }

        // Copy the files to proper location, but skip existing files
        foreach(var file in GetFiles("./.cake-working/**/*.*"))
        {
            var newFilePath = new FilePath(file.FullPath.Replace("/.cake-working", ""));
            if (!FileExists(newFilePath))
            {
                CreateDirectory(newFilePath.GetDirectory());
                CopyFile(file, newFilePath);
                Information($"Created {newFilePath}");
            }
        }

        // Delete the temp working dirs & files
        DeleteDirectory(workingDir, new DeleteDirectorySettings { Recursive = true, Force = true });

        Information($"Files for day {day} now exist.");
    });

Task("CreateAllDays")
    .Does(() =>
    {
        for(var dayCounter = 0; dayCounter <= 25; dayCounter++)
        {
            day = dayCounter.ToString();
            RunTarget("CreateDay");
        }
    });

RunTarget(target);

using Microsoft.Extensions.Configuration;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using static System.Environment;

namespace AoC.Tests;

[AttributeUsage(AttributeTargets.Method)]
public class LongRunningTestAttribute : Attribute, IApplyToTest
{
    private const string IncludeLongRunningTests = nameof(IncludeLongRunningTests);

    public string RoughDuration { get; }

    public string FullMessage { get; }

    public static bool IsIncludeLongRunningTests => Config.Value[IncludeLongRunningTests] == "true";

    public static readonly string DefaultConfigFilePath =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.tests.json");

    public static readonly string UserConfigFilePath =
        Path.Combine(GetFolderPath(SpecialFolder.UserProfile), ".aoc", "config.tests.json");

    private static readonly Lazy<IConfiguration> Config = new(() => new ConfigurationBuilder()
        .AddJsonFile(DefaultConfigFilePath, optional: true)
        .AddJsonFile(UserConfigFilePath, optional: true)
        .AddEnvironmentVariables()
        .Build());

    public LongRunningTestAttribute(string roughDuration)
    {
        RoughDuration = roughDuration;
        FullMessage =
            $"This test has been marked as long running ({RoughDuration}), and long running tests are currently excluded.{NewLine}{NewLine}"
            + $"To include long running tests, set the `{IncludeLongRunningTests}` config value to `true`.{NewLine}"
            + $"This can be done via an environment variable, or via one of the config files:{NewLine}"
            + $"{DefaultConfigFilePath}{NewLine}{UserConfigFilePath}";
    }

    /// <summary>
    /// Modifies a test by marking it as Ignored, unless long running test are enabled in this test run.
    /// </summary>
    /// <param name="test">The test to modify</param>
    public void ApplyToTest(Test test)
    {
        if (IsIncludeLongRunningTests)
        {
            return;
        }

        test.RunState = RunState.Ignored;
        test.Properties.Set(PropertyNames.SkipReason, FullMessage);
    }
}

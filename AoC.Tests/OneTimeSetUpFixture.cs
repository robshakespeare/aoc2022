namespace AoC.Tests;

[SetUpFixture]
public static class OneTimeSetUpFixture
{
    /// <summary>
    /// Ran only once, before all tests, but after the test discovery phase.
    /// </summary>
    [OneTimeSetUp]
    public static void RunOnceBeforeAllTests()
    {
        Crayon.Output.Disable();
    }
}

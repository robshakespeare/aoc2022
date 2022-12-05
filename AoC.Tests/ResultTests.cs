namespace AoC.Tests;

public class ResultTests
{
    [Test]
    public void DefaultValue_Test()
    {
        // ACT
        var sut = new Result();

        // ASSERT
        sut.Should().BeEquivalentTo(new
        {
            Value = (object?) null,
            Elapsed = (TimeSpan?) null,
            IsStarted = false,
            IsCompleted = false,
            IsRunning = false,
            ElapsedTotalSeconds = 0,
            IsCompletedWithValue = false,
            IsCompletedWithoutValue = false
        });
    }

    [Test]
    public void Started_Test()
    {
        // ACT
        var sut = Result.Started();

        // ASSERT
        sut.Should().BeEquivalentTo(new
        {
            Value = (object?)null,
            Elapsed = (TimeSpan?)null,
            IsStarted = true,
            IsCompleted = false,
            IsRunning = true,
            ElapsedTotalSeconds = 0,
            IsCompletedWithValue = false,
            IsCompletedWithoutValue = false
        });
    }

    [TestCase("Hello world")]
    [TestCase(12345L)]
    public void Started_WithValue_Tests(object value)
    {
        // ACT
        var sut = Result.Completed(value, TimeSpan.FromSeconds(0.654));

        // ASSERT
        sut.Should().BeEquivalentTo(new
        {
            Value = value,
            Elapsed = TimeSpan.FromMilliseconds(654),
            IsStarted = true,
            IsCompleted = true,
            IsRunning = false,
            ElapsedTotalSeconds = 0.654,
            IsCompletedWithValue = true,
            IsCompletedWithoutValue = false
        });
    }

    [Test]
    public void Started_WithoutValue_Test()
    {
        // ACT
        var sut = Result.Completed(null, TimeSpan.FromSeconds(0.018));

        // ASSERT
        sut.Should().BeEquivalentTo(new
        {
            Value = (object?) null,
            Elapsed = TimeSpan.FromMilliseconds(18),
            IsStarted = true,
            IsCompleted = true,
            IsRunning = false,
            ElapsedTotalSeconds = 0.018,
            IsCompletedWithValue = false,
            IsCompletedWithoutValue = true
        });
    }
}

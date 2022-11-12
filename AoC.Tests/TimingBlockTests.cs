namespace AoC.Tests;

public class TimingBlockTests
{
    [Test]
    public async Task TimingBlock_Stop_ReturnsElapsedTime()
    {
        using var sut = new TimingBlock("sut");
        await Task.Delay(30);

        // ACT
        var result = sut.Stop();

        // ASSERT
        result.Should().BeGreaterThan(TimeSpan.FromMilliseconds(15));
    }
}

using AoC.Day00;

namespace AoC.Tests.Day00;

public class Day0SolverTests
{
    private readonly Day0Solver _sut = new();

    [Test]
    public void Part1Example()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(@"12
127");

        // ASSERT
        part1ExampleResult.Should().Be(1524);
    }

    [Test]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(136);
        _sut.Part1Result!.Value.Should().Be(136);
        _sut.Part1Result.IsStarted.Should().BeTrue();
        _sut.Part1Result.IsCompleted.Should().BeTrue();
    }

    [Test]
    public void Part2Example()
    {
        // ACT
        var part2ExampleResult = _sut.SolvePart2("8,18,24");

        // ASSERT
        part2ExampleResult.Should().Be(72);
    }

    [Test]
    public void Part2ReTest()
    {
        // ACT
        var part2Result = _sut.SolvePart2();

        // ASSERT
        part2Result.Should().Be(840);
        _sut.Part2Result!.Value.Should().Be(840);
        _sut.Part2Result.IsStarted.Should().BeTrue();
        _sut.Part2Result.IsCompleted.Should().BeTrue();
    }
}

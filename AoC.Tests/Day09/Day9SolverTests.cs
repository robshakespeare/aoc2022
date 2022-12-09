using AoC.Day09;

namespace AoC.Tests.Day09;

public class Day9SolverTests
{
    private readonly Day9Solver _sut = new();

    private const string ExampleInput = @"R 4
U 4
L 3
D 1
R 4
D 1
L 5
R 2";

    [Test]
    public void Part1Example()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(ExampleInput);

        // ASSERT
        part1ExampleResult.Should().Be(13);
    }

    [Test]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().NotBe(7015);
        part1Result.Should().Be(6464);
    }

    [Test]
    public void Part2Example1()
    {
        // ACT
        var part2ExampleResult = _sut.SolvePart2(ExampleInput);

        // ASSERT
        part2ExampleResult.Should().Be(1);
    }

    [Test]
    public void Part2Example2()
    {
        // ACT
        var part2ExampleResult = _sut.SolvePart2(@"R 5
U 8
L 8
D 3
R 17
D 10
L 25
U 20");

        // ASSERT
        part2ExampleResult.Should().Be(36);
    }

    [Test]
    public void Part2ReTest()
    {
        // ACT
        var part2Result = _sut.SolvePart2();

        // ASSERT
        part2Result.Should().Be(2604);
    }
}

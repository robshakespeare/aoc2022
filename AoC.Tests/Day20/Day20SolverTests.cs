using AoC.Day20;

namespace AoC.Tests.Day20;

public class Day20SolverTests
{
    private readonly Day20Solver _sut = new();

    private const string ExampleInput = """
        1
        2
        -3
        3
        -2
        0
        4
        """;

    [Test]
    public void Part1Example()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(ExampleInput);

        // ASSERT
        part1ExampleResult.Should().Be(3);
    }

    [Test]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(7713);
    }

    [Test]
    public void Part2Example()
    {
        // ACT
        var part2ExampleResult = _sut.SolvePart2(ExampleInput);

        // ASSERT
        part2ExampleResult.Should().Be(1623178306);
    }

    [Test]
    public void Part2ReTest()
    {
        // ACT
        var part2Result = _sut.SolvePart2();

        // ASSERT
        part2Result.Should().Be(null);
    }

    public class TheDecryptMethod
    {
        [TestCase("1, 2, -3, 3, -2, 0, 4", "1, 2, -3, 4, 0, 3, -2")]
        [TestCase("-1, 0, 0", "0, 0, -1")]
        [TestCase("-2, 0, 0", "0, -2, 0")]
        [TestCase("0, 0, 1", "1, 0, 0")]
        [TestCase("0, 0, 2", "0, 2, 0")]
        [TestCase("-1, 0, 0, 0", "0, 0, 0, -1")]
        [TestCase("-2, 0, 0, 0", "0, 0, -2, 0")]
        [TestCase("0, 0, 0, 1", "1, 0, 0, 0")]
        [TestCase("0, 0, 0, 2", "0, 2, 0, 0")]
        [TestCase("0, 0, 3", "3, 0, 0")]
        [TestCase("0, 0, 4", "0, 4, 0")]
        [TestCase("-3, 0, 0", "0, 0, -3")]
        [TestCase("-4, 0, 0", "0, -4, 0")]
        [TestCase("0, 0, 10", "0, 10, 0")]
        [TestCase("0, 0, 11", "11, 0, 0")]
        [TestCase("0, 0, -10", "0, 0, -10")]
        [TestCase("0, 0, -11", "0, -11, 0")]
        [TestCase("10, 0, 0 ", "10, 0, 0")]
        [TestCase("11, 0, 0", "0, 11, 0")]
        [TestCase("-10, 0, 0", "0, -10, 0")]
        [TestCase("-11, 0, 0", "0, 0, -11")]
        [TestCase("303, 2, 3, 4, 5", "5, 4, 2, 303, 3")]
        [TestCase("-303, 2, 3, 4, 5", "5, -303, 2, 4, 3")]
        [TestCase("1, 303, 3, 4, 5", "5, 3, 1, 4, 303")]
        [TestCase("1, -303, 3, 4, 5", "5, 3, 1, -303, 4")]
        [TestCase("1, 2, 3, 4, 303", "3, 1, 303, 2, 4")]
        [TestCase("1, 2, 3, 4, -303", "3, -303, 1, 2, 4")]
        [TestCase("1, 2, 3, 303, 5", "5, 3, 1, 303, 2")]
        [TestCase("1, 2, 3, -303, 5", "5, 3, 1, 2, -303")]
        public void Decrypt_SingleCycle_Tests(string input, string expected)
        {
            var inputLines = input.Replace(", ", Environment.NewLine);

            // ACT
            var result = Day20Solver.Decrypt(inputLines);

            // ASSERT
            string.Join(", ", result).Should().Be(expected);
        }
    }
}

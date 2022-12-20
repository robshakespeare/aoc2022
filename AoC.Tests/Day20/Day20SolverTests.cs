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
    [Ignore("rs-todo")]
    public void Part2Example()
    {
        // ACT
        var part2ExampleResult = _sut.SolvePart2(ExampleInput);

        // ASSERT
        part2ExampleResult.Should().Be(1623178306);
    }

    [Test]
    [Ignore("rs-todo")]
    public void Part2ReTest()
    {
        // ACT
        var part2Result = _sut.SolvePart2();

        // ASSERT
        part2Result.Should().NotBe(-9808);
        part2Result.Should().BeOfType<long>().Which.Should().BeLessThan(2086595712363);

        part2Result.Should().Be(null);
    }

    public class TheMoveNumberMethod
    {
        //[TestCase("4, 5, 6, 1, 7, 8, 9", 1, "4, 5, 6, 7, 1, 8, 9")] // Example A in paragraph
        //[TestCase("4, -2, 5, 6, 7, 8, 9", -2, "4, 5, 6, 7, 8, -2, 9")] // Example B in paragraph
        //[TestCase("4, 5, 6, 7, 8, 2, 9", 2, "4, 2, 5, 6, 7, 8, 9")] // Self-changed paragraph example

        //[TestCase("1, 2, -3, 3, -2, 0, 4", 1, "2, 1, -3, 3, -2, 0, 4")] // Example 1, step 1
        //[TestCase("2, 1, -3, 3, -2, 0, 4", 2, "1, -3, 2, 3, -2, 0, 4")] // Example 1, step 2
        //[TestCase("1, -3, 2, 3, -2, 0, 4", -3, "1, 2, 3, -2, -3, 0, 4")] // Example 1, step 3
        //[TestCase("1, 2, 3, -2, -3, 0, 4", 3, "1, 2, -2, -3, 0, 3, 4")] // Example 1, step 4
        //[TestCase("1, 2, -2, -3, 0, 3, 4", -2, "1, 2, -3, 0, 3, 4, -2")] // Example 1, step 5
        //[TestCase("1, 2, -3, 0, 3, 4, -2", 0, "1, 2, -3, 0, 3, 4, -2")] // Example 1, step 6
        //[TestCase("1, 2, -3, 0, 3, 4, -2", 4, "1, 2, -3, 4, 0, 3, -2")] // Example 1, step 7

        //[TestCase("4, 5, 6, 1", 1, "4, 1, 5, 6")] // My example 1
        //[TestCase("7, 8, 9, 2", 2, "7, 8, 2, 9")] // My example 2
        //public void MoveNumber_ExampleTests(string input, int numberToMove, string expected)
        //{
        //    var numbers = input.Split(", ").Select(x => new Day20Solver.Number(int.Parse(x))).ToList();
        //    var number = numbers.Single(x => x.Value == numberToMove);

        //    // ACT
        //    Day20Solver.MoveNumber(number, numbers);

        //    // ASSERT
        //    string.Join(", ", numbers).Should().Be(expected);
        //}

        [TestCase("4, 5, 6, 1, 7, 8, 9", 1, "4, 5, 6, 7, 1, 8, 9")] // Example A in paragraph
        [TestCase("4, -2, 5, 6, 7, 8, 9", -2, "4, 5, 6, 7, 8, -2, 9")] // Example B in paragraph
        [TestCase("4, 5, 6, 7, 8, 2, 9", 2, "4, 2, 5, 6, 7, 8, 9")] // Self-changed paragraph example

        [TestCase("1, 2, -3, 3, -2, 0, 4", 1, "2, 1, -3, 3, -2, 0, 4")] // Example 1, step 1
        [TestCase("2, 1, -3, 3, -2, 0, 4", 2, "1, -3, 2, 3, -2, 0, 4")] // Example 1, step 2
        [TestCase("1, -3, 2, 3, -2, 0, 4", -3, "1, 2, 3, -2, -3, 0, 4")] // Example 1, step 3
        [TestCase("1, 2, 3, -2, -3, 0, 4", 3, "1, 2, -2, -3, 0, 3, 4")] // Example 1, step 4
        [TestCase("1, 2, -2, -3, 0, 3, 4", -2, "1, 2, -3, 0, 3, 4, -2")] // Example 1, step 5
        [TestCase("1, 2, -3, 0, 3, 4, -2", 0, "1, 2, -3, 0, 3, 4, -2")] // Example 1, step 6
        [TestCase("1, 2, -3, 0, 3, 4, -2", 4, "1, 2, -3, 4, 0, 3, -2")] // Example 1, step 7

        [TestCase("4, 5, 6, 1", 1, "4, 1, 5, 6")] // My example 1
        [TestCase("7, 8, 9, 2", 2, "7, 8, 2, 9")] // My example 2
        public void MoveNumberNode_ExampleTests(string input, int numberToMove, string expected)
        {
            //var numbers = input.Split(", ").Select(x => new Day20Solver.Number(int.Parse(x))).ToList();
            var encrypted = new LinkedList<long>(input.Split(", ").Select(long.Parse));
            var number = Day20Solver.EnumerateNodes(encrypted).Single(x => x.Value == numberToMove);

            // ACT
            Day20Solver.MoveNumberNode(number);

            // ASSERT
            string.Join(", ", encrypted).Should().Be(expected);
        }
    }

    public class TheDecryptMethod
    {
        [TestCase("1, 2, -3, 3, -2, 0, 4", "1, 2, -3, 4, 0, 3, -2")]

        //[TestCase("-1, 0, 0", "0, -1, 0")]
        //[TestCase("-2, 0, 0", "0, 0, -2")]
        //[TestCase("0, 0, 1", "1, 0, 0")]
        //[TestCase("0, 0, 2", "0, 2, 0")]
        //[TestCase("0, 0, 3", "3, 0, 0")]
        //[TestCase("0, 0, 4", "0, 4, 0")]
        //[TestCase("-3, 0, 0", "0, 0, -3")]
        //[TestCase("-4, 0, 0", "0, -4, 0")]
        //[TestCase("0, 0, 10", "0, 10, 0")]
        //[TestCase("0, 0, 11", "11, 0, 0")]
        //[TestCase("0, 0, -10", "0, 0, -10")]
        //[TestCase("0, 0, -11", "0, -11, 0")]
        //[TestCase("10, 0, 0", "10, 0, 0")]
        //[TestCase("11, 0, 0", "0, 11, 0")]
        //[TestCase("-10, 0, 0", "0, -10, 0")]
        //[TestCase("-11, 0, 0", "0, 0, -11")]

        [TestCase("-1, 0, 0, 0", "0, 0, -1, 0")]
        [TestCase("-2, 0, 0, 0", "0, -2, 0, 0")]
        [TestCase("0, 0, 0, 1", "0, 1, 0, 0")]
        [TestCase("0, 0, 0, 2", "0, 0, 2, 0")]
        
        
        [TestCase("303, 2, 3, 4, 5", "4, 5, 2, 303, 3")]
        [TestCase("-303, 2, 3, 4, 5", "3, 5, -303, 2, 4")]
        [TestCase("1, 303, 3, 4, 5", "3, 5, 1, 4, 303")]
        [TestCase("1, -303, 3, 4, 5", "1, 5, 3, -303, 4")]
        [TestCase("1, 2, 3, 4, 303", "3, 1, 2, 303, 4")]
        [TestCase("1, 2, 3, 4, -303", "3, -303, 1, 2, 4")]
        [TestCase("1, 2, 3, 303, 5", "3, 5, 1, 303, 2")]
        [TestCase("1, 2, 3, -303, 5", "5, 3, 1, 2, -303")]
        [TestCase("10, 0, 0, 0", "0, 10, 0, 0")]
        [TestCase("11, 0, 0, 0", "0, 0, 11, 0")]
        [TestCase("-7, 1, 2, 3", "3, 1, -7, 2")]
        [TestCase("-7, 0, 0, 0", "0, 0, -7, 0")]
        [TestCase("-8, 0, 0, 0", "0, -8, 0, 0")]
        [TestCase("-9, 0, 0, 0", "0, 0, 0, -9")]
        [TestCase("0, -9, 0, 0", "0, -9, 0, 0")]
        [TestCase("-10, 0, 0, 0", "0, 0, -10, 0")]
        [TestCase("-11, 0, 0, 0", "0, -11, 0, 0")]
        [TestCase("-12, 0, 0, 0", "0, 0, 0, -12")]
        [TestCase("9153, 8306, -2434, 7459, -8306, 0, 6612", "6612, 8306, 7459, -8306, -2434, 9153, 0")]
        public void Decrypt_SingleCycle_Tests(string input, string expected)
        {
            Decrypt_SingleCycle_Test(input, expected);

            //var inputLines = input.Replace(", ", Environment.NewLine);

            //// ACT
            //var result = Day20Solver.Decrypt(inputLines);

            //// ASSERT
            //string.Join(", ", result).Should().Be(expected);
        }

        [Test]
        [Ignore("rs-todo!")]
        public void Decrypt_LargeNumbersExample()
        {
            const string input = "811589153, 1623178306, -2434767459, 2434767459, -1623178306, 0, 3246356612";
            const string expected = "0, -2434767459, 3246356612, -1623178306, 2434767459, 1623178306, 811589153";

            Decrypt_SingleCycle_Test(input, expected);
        }

        private static void Decrypt_SingleCycle_Test(string input, string expected)
        {
            var inputLines = input.Replace(", ", Environment.NewLine);

            // ACT
            var result = Day20Solver.Decrypt(inputLines);

            // ASSERT
            string.Join(", ", result).Should().Be(expected);
        }
    }
}

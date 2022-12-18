using AoC.Day18;
using static AoC.Day18.Day18Solver;

namespace AoC.Tests.Day18;

public class Day18SolverTests
{
    private readonly Day18Solver _sut = new()
    {
        Logger = TestContext.Progress.WriteLine
    };

    private const string ExampleInput = """
        2,2,2
        1,2,2
        3,2,2
        2,1,2
        2,3,2
        2,2,1
        2,2,3
        2,2,4
        2,2,6
        1,2,5
        3,2,5
        2,1,5
        2,3,5
        """;

    [Test]
    public void Part1Example()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(ExampleInput);

        // ASSERT
        part1ExampleResult.Should().Be(64);
    }

    [Test]
    public void Part1TinyExample()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1("""
            1,1,1
            2,1,1
            """);

        // ASSERT
        part1ExampleResult.Should().Be(10);
    }

    [Test]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(4604);
    }

    [Test]
    public void Part2Example()
    {
        // ACT
        var part2ExampleResult = _sut.SolvePart2(ExampleInput);

        // ASSERT
        part2ExampleResult.Should().Be(58);
    }

    [Test]
    public void Part2ReTest()
    {
        // ACT
        var part2Result = _sut.SolvePart2();

        // ASSERT
        var part2ResultLong = part2Result.Should().BeOfType<long>().Which;
        part2ResultLong.Should().BeLessThan(3694);

        part2Result.Should().NotBe(1651);
        part2Result.Should().NotBe(3070);
        part2Result.Should().NotBe(3694);
        part2Result.Should().NotBe(2044); // but is it 2044 + 8? i.e. 2,052
        part2Result.Should().NotBe(-4366);
        part2Result.Should().NotBe(null);

        part2Result.Should().Be(-1);
    }

    public class TheAreDirectlyFacingMethod
    {
        [Test]
        public void AreDirectlyFacing_AreDirectlyFacingTowardsEachOther_ShouldReturnTrue()
        {
            var surface1 = new Surface(new(0, 0, 0), new(1, 0, 0));
            var surface2 = new Surface(new(4, 0, 0), new(-1, 0, 0));

            // ACT
            var areDirectlyFacing = AreDirectlyFacing(surface1, surface2);

            // ASSERT
            areDirectlyFacing.Should().BeTrue();
        }

        [Test]
        public void AreDirectlyFacing_AreDirectlyFacingTowardsEachOther_OtherDirection_ShouldReturnTrue()
        {
            var surface1 = new Surface(new(4, 0, 0), new(-1, 0, 0));
            var surface2 = new Surface(new(0, 0, 0), new(1, 0, 0));

            // ACT
            var areDirectlyFacing = AreDirectlyFacing(surface1, surface2);

            // ASSERT
            areDirectlyFacing.Should().BeTrue();
        }

        [Test]
        public void AreDirectlyFacing_AreDirectlyFacingTowardsEachOther_ButOnSamePlane_ShouldReturnFalse()
        {
            var surface1 = new Surface(new(0, 0, 0), new(1, 0, 0));
            var surface2 = new Surface(new(0, 0, 0), new(-1, 0, 0));

            // ACT
            var areDirectlyFacing = AreDirectlyFacing(surface1, surface2);

            // ASSERT
            areDirectlyFacing.Should().BeFalse();
        }

        [Test]
        public void AreDirectlyFacing_AreDirectlyFacingTowardsEachOther_OnlyOneAway_ShouldReturnTrue()
        {
            var surface1 = new Surface(new(0, 0, 0), new(1, 0, 0));
            var surface2 = new Surface(new(1, 0, 0), new(-1, 0, 0));

            // ACT
            var areDirectlyFacing = AreDirectlyFacing(surface1, surface2);

            // ASSERT
            areDirectlyFacing.Should().BeTrue();
        }

        [Test]
        public void AreDirectlyFacing_AreDirectlyFacingAwayFromEachOther_ShouldReturnFalse()
        {
            var surface1 = new Surface(new(0, 0, 0), new(1, 0, 0));
            var surface2 = new Surface(new(4, 0, 0), new(1, 0, 0));

            // ACT
            var areDirectlyFacing = AreDirectlyFacing(surface1, surface2);

            // ASSERT
            areDirectlyFacing.Should().BeFalse();
        }

        [Test]
        public void AreDirectlyFacing_AreFacingTowardsEachOther_ButNotDirectly_WithTouchingEdge_ShouldReturnFalse()
        {
            var surface1 = new Surface(new(0, 0, 0), new(1, 0, 0));
            var surface2 = new Surface(new(4, 1, 0), new(-1, 0, 0));

            // ACT
            var areDirectlyFacing = AreDirectlyFacing(surface1, surface2);

            // ASSERT
            areDirectlyFacing.Should().BeFalse();
        }

        [Test]
        public void AreDirectlyFacing_AreFacingTowardsEachOther_ButNotDirectly_WithoutTouchingEdge_ShouldReturnFalse()
        {
            var surface1 = new Surface(new(0, 0, 0), new(1, 0, 0));
            var surface2 = new Surface(new(4, 5, 5), new(-1, 0, 0));

            // ACT
            var areDirectlyFacing = AreDirectlyFacing(surface1, surface2);

            // ASSERT
            areDirectlyFacing.Should().BeFalse();
        }

        [Test]
        public void AreDirectlyFacing_AreFacingDifferentDirections_ShouldReturnFalse()
        {
            var surface1 = new Surface(new(0, 0, 0), new(1, 0, 0));
            var surface2 = new Surface(new(0, 0, 0), new(0, -1, 0));

            // ACT
            var areDirectlyFacing = AreDirectlyFacing(surface1, surface2);

            // ASSERT
            areDirectlyFacing.Should().BeFalse();
        }

        //[Test]
        //public void AreDirectlyFacing_AreDirectlyFacingTowardsEachOther_ShouldReturnTrue()
        //{
        //    var surface1 = new Surface(new(0, 0, 0), new(0, 1, 1), new(1, 0, 0));
        //    var surface2 = new Surface(new(4, 0, 0), new(4, 1, 1), new(-1, 0, 0));

        //    // ACT
        //    var areDirectlyFacing = AreDirectlyFacing(surface1, surface2);

        //    // ASSERT
        //    areDirectlyFacing.Should().BeTrue();
        //}

        //[Test]
        //public void AreDirectlyFacing_AreDirectlyFacingTowardsEachOther_OtherDirection_ShouldReturnTrue()
        //{
        //    var surface1 = new Surface(new(4, 0, 0), new(4, 1, 1), new(-1, 0, 0));
        //    var surface2 = new Surface(new(0, 0, 0), new(0, 1, 1), new(1, 0, 0));

        //    // ACT
        //    var areDirectlyFacing = AreDirectlyFacing(surface1, surface2);

        //    // ASSERT
        //    areDirectlyFacing.Should().BeTrue();
        //}

        //[Test]
        //public void AreDirectlyFacing_AreDirectlyFacingTowardsEachOther_ButOnSamePlane_ShouldReturnFalse()
        //{
        //    var surface1 = new Surface(new(0, 0, 0), new(0, 1, 1), new(1, 0, 0));
        //    var surface2 = new Surface(new(0, 0, 0), new(0, 1, 1), new(-1, 0, 0));

        //    // ACT
        //    var areDirectlyFacing = AreDirectlyFacing(surface1, surface2);

        //    // ASSERT
        //    areDirectlyFacing.Should().BeFalse();
        //}

        //[Test]
        //public void AreDirectlyFacing_AreDirectlyFacingTowardsEachOther_OnlyOneAway_ShouldReturnTrue()
        //{
        //    var surface1 = new Surface(new(0, 0, 0), new(0, 1, 1), new(1, 0, 0));
        //    var surface2 = new Surface(new(1, 0, 0), new(1, 1, 1), new(-1, 0, 0));

        //    // ACT
        //    var areDirectlyFacing = AreDirectlyFacing(surface1, surface2);

        //    // ASSERT
        //    areDirectlyFacing.Should().BeTrue();
        //}

        //[Test]
        //public void AreDirectlyFacing_AreDirectlyFacingAwayFromEachOther_ShouldReturnFalse()
        //{
        //    var surface1 = new Surface(new(0, 0, 0), new(0, 1, 1), new(1, 0, 0));
        //    var surface2 = new Surface(new(4, 0, 0), new(4, 1, 1), new(1, 0, 0));

        //    // ACT
        //    var areDirectlyFacing = AreDirectlyFacing(surface1, surface2);

        //    // ASSERT
        //    areDirectlyFacing.Should().BeFalse();
        //}

        //[Test]
        //public void AreDirectlyFacing_AreFacingTowardsEachOther_ButNotDirectly_WithTouchingEdge_ShouldReturnFalse()
        //{
        //    var surface1 = new Surface(new(0, 0, 0), new(0, 1, 1), new(1, 0, 0));
        //    var surface2 = new Surface(new(4, 1, 0), new(4, 2, 1), new(-1, 0, 0));

        //    // ACT
        //    var areDirectlyFacing = AreDirectlyFacing(surface1, surface2);

        //    // ASSERT
        //    areDirectlyFacing.Should().BeFalse();
        //}

        //[Test]
        //public void AreDirectlyFacing_AreFacingTowardsEachOther_ButNotDirectly_WithoutTouchingEdge_ShouldReturnFalse()
        //{
        //    var surface1 = new Surface(new(0, 0, 0), new(0, 1, 1), new(1, 0, 0));
        //    var surface2 = new Surface(new(4, 5, 5), new(4, 6, 6), new(-1, 0, 0));

        //    // ACT
        //    var areDirectlyFacing = AreDirectlyFacing(surface1, surface2);

        //    // ASSERT
        //    areDirectlyFacing.Should().BeFalse();
        //}

        //[Test]
        //public void AreDirectlyFacing_AreFacingDifferentDirections_ShouldReturnFalse()
        //{
        //    var surface1 = new Surface(new(0, 0, 0), new(0, 1, 1), new(1, 0, 0));
        //    var surface2 = new Surface(new(0, 0, 0), new(1, 0, 1), new(0, -1, 0));

        //    // ACT
        //    var areDirectlyFacing = AreDirectlyFacing(surface1, surface2);

        //    // ASSERT
        //    areDirectlyFacing.Should().BeFalse();
        //}
    }
}

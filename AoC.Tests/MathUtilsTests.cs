namespace AoC.Tests;

public class MathUtilsTests
{
    public class TheLeastCommonMultipleMethod
    {
        [Test]
        public void LeastCommonMultiple_With2Arguments_ReturnsExpectedResult()
        {
            MathUtils.LeastCommonMultiple(28, 63).Should().Be(252);
        }

        [Test]
        public void LeastCommonMultiple_With3Arguments_ReturnsExpectedResult()
        {
            MathUtils.LeastCommonMultiple(12, 15, 18).Should().Be(180);
        }
    }

    public class TheAngleBetweenMethod
    {
        [TestCase(0, 1, 0)]
        [TestCase(1, 1, 315)]
        [TestCase(1, 0, 270)]
        [TestCase(1, -1, 225)]
        [TestCase(0, -1, 180)]
        [TestCase(-1, -1, 135)]
        [TestCase(-1, 0, 90)]
        [TestCase(-1, 1, 45)]
        public void AngleBetween_Tests(int x, int y, int expectedAngle)
        {
            var angles = Enumerable
                .Range(1, 5)
                .Select(n => new Vector2(x * n * 100, y * n * 100))
                .Select(v =>
                {
                    Console.WriteLine(v);
                    return MathUtils.AngleBetween(Vector2.UnitY, v);
                })
                .ToArray();

            angles.Should().AllBeEquivalentTo(expectedAngle);
        }
    }

    public class TheRotateDirectionMethod
    {
        [Test]
        public void Rotate_East_90DegreesToRight_ShouldBe_South()
        {
            var input = GridUtils.East;

            // ACT
            var result = MathUtils.RotateDirection(input, 90);

            // ASSERT
            result.Should().Be(GridUtils.South);
        }

        [Test]
        public void Rotate_East_360DegreesToRight_ShouldBe_Still_East()
        {
            var input = GridUtils.East;

            // ACT
            var result = MathUtils.RotateDirection(input, 360);

            // ASSERT
            result.Should().Be(GridUtils.East);
        }

        [Test]
        public void Rotate_East_90DegreesToLeft_ShouldBe_North()
        {
            var input = GridUtils.East;

            // ACT
            var result = MathUtils.RotateDirection(input, -90);

            // ASSERT
            result.Should().Be(GridUtils.North);
        }

        [Test]
        public void Rotate_North_180DegreesToLeft_ShouldBe_South()
        {
            var input = GridUtils.North;

            // ACT
            var result = MathUtils.RotateDirection(input, -180);

            // ASSERT
            result.Should().Be(GridUtils.South);
        }

        [Test]
        public void Rotate_North_180DegreesToRight_ShouldBe_South()
        {
            var input = GridUtils.North;

            // ACT
            var result = MathUtils.RotateDirection(input, 180);

            // ASSERT
            result.Should().Be(GridUtils.South);
        }

        [Test]
        public void Rotate_South_270DegreesToRight_ShouldBe_East()
        {
            var input = GridUtils.South;

            // ACT
            var result = MathUtils.RotateDirection(input, 270);

            // ASSERT
            result.Should().Be(GridUtils.East);
        }

        [Test]
        public void Rotate_South_270DegreesToLeft_ShouldBe_West()
        {
            var input = GridUtils.South;

            // ACT
            var result = MathUtils.RotateDirection(input, -270);

            // ASSERT
            result.Should().Be(GridUtils.West);
        }

        [Test]
        public void Rotate_Waypoint_Day12_Example1_Test()
        {
            var input = new Vector2(10, -4);

            // ACT
            var result = MathUtils.RotateDirection(input, 90);

            // ASSERT
            result.Should().Be(new Vector2(4, 10));
        }
    }

    public class TheRoundMethod
    {
        [TestCase(1.0f, 1)]
        [TestCase(2.0f, 2)]
        [TestCase(-1.0f, -1)]
        [TestCase(3.25f, 3)]
        [TestCase(3.75f, 4)]
        [TestCase(3.5f, 4)]
        [TestCase(4.5f, 5)]
        [TestCase(1234.00001f, 1234)]
        [TestCase(1234.49f, 1234)]
        [TestCase(1234.5f, 1235)]
        [TestCase(1234.9999f, 1235)]
        [TestCase(9345678.75f, 9345679)]
        public void RoundTests(float f, int expectedResult)
        {
            f.Round().Should().Be(expectedResult);
        }
    }

    public class ManhattanDistanceTests
    {
        [TestCase(0, 0, 0, 0, 0)]
        [TestCase(0, 0, 0, 1, 1)]
        [TestCase(0, 0, 0, -1, 1)]
        [TestCase(0, 0, 2, 3, 5)]
        [TestCase(0, 0, -2, -3, 5)]
        [TestCase(1, 6, -1, 5, 3)]
        [TestCase(2, 3, -1, 5, 5)]
        [TestCase(2, 3, 1, 6, 4)]
        [TestCase(2, 3, 3, 5, 3)]
        public void ManhattanDistance_Tests(int x1, int y1, int x2, int y2, int expectedResult)
        {
            var vectorA = new Vector2(x1, y1);
            var vectorB = new Vector2(x2, y2);

            // ACT
            var result = MathUtils.ManhattanDistance(vectorA, vectorB);

            // ASSERT
            result.Should().Be(expectedResult);
        }

        [Test]
        public void ManhattanDistance_DoesRound_BeforeCalculating()
        {
            var vectorA = new Vector2(1.2f, 1.4f);
            var vectorB = new Vector2(3.5f, 4.5f);

            // ACT
            var result = MathUtils.ManhattanDistance(vectorA, vectorB);

            // ASSERT
            result.Should().Be((4 - 1) + (5 - 1));
        }

        [TestCase(0, 0, 0)]
        [TestCase(0, 1, 1)]
        [TestCase(0, -1, 1)]
        [TestCase(2, 3, 5)]
        [TestCase(-2, -3, 5)]
        [TestCase(10, 16, 26)]
        [TestCase(-10, 16, 26)]
        [TestCase(10, -16, 26)]
        [TestCase(-10, -16, 26)]
        public void ManhattanDistanceFromZero_Tests(int x, int y, int expectedResult)
        {
            var vector = new Vector2(x, y);

            // ACT
            var result = vector.ManhattanDistanceFromZero();

            // ASSERT
            result.Should().Be(expectedResult);
        }

        [Test]
        public void ManhattanDistance_3D_Test()
        {
            var vectorA = new Vector3(1105, -1205, 1229);
            var vectorB = new Vector3(-92, -2380, -20);

            // ACT
            var result = MathUtils.ManhattanDistance(vectorA, vectorB);

            // ASSERT
            result.Should().Be(3621);
        }
    }

    public class TheMedianMethod
    {
        [TestCase("12,3,5", ExpectedResult = 5)]
        [TestCase("5,9,3,7", ExpectedResult = 6)]
        [TestCase("15,18,1,30", ExpectedResult = 16)]
        [TestCase("3,13,2,34,11,17,27,47", ExpectedResult = 15)]
        [TestCase("3,13,7,5,21,23,39,23,40,23,14,12,56,23,29", ExpectedResult = 23)]
        [TestCase("3,13,7,5,21,23,23,40,23,14,12,56,23,29", ExpectedResult = 22)]
        [TestCase("1,6,8,4,1,-5,-3,0,1,-1,-2", ExpectedResult = 1)]
        [TestCase("12,3", ExpectedResult = 7)]
        [TestCase("8", ExpectedResult = 8)]
        public long MedianTests(string values) => values.Split(',').Select(long.Parse).Median();

        [Test]
        public void Median_OfEmptyCollection_ThrowsExpectedEx()
        {
            var action = () => Array.Empty<long>().Median();

            // ACT & ASSERT
            action.Should().Throw<InvalidOperationException>().WithMessage("The median of an empty data set is undefined.");
        }
    }
}

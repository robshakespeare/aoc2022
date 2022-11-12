using FluentAssertions.Equivalency;

namespace AoC.Tests;

public class GridUtilsTests
{
    private static EquivalencyAssertionOptions<T> WithStrictOrdering<T>(EquivalencyAssertionOptions<T> options) => options.WithStrictOrdering();

    public class TheCenterAndDirectionsIncludingDiagonalProperty
    {
        [Test]
        public void CenterAndDirectionsIncludingDiagonal_ShouldReturnsExpectedValuesInExpectedOrder()
        {
            GridUtils.CenterAndDirectionsIncludingDiagonal.Should().BeEquivalentTo(new[]
            {
                new Vector2(-1, -1),
                new Vector2(0, -1),
                new Vector2(1, -1),

                new Vector2(-1, 0),
                new Vector2(0, 0),
                new Vector2(1, 0),

                new Vector2(-1, 1),
                new Vector2(0, 1),
                new Vector2(1, 1)
            }, WithStrictOrdering);
        }
    }

    public class TheRotateGridMethod
    {
        [Test]
        public void RotateGrid_SimpleTest()
        {
            var input = new[]
            {
                "12",
                "34"
            };

            // ACT
            var result = GridUtils.RotateGrid(input, 90);

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    "31",
                    "42"
                },
                WithStrictOrdering);
        }

        [Test]
        public void RotateGrid_3x3_0deg()
        {
            var input = new[]
            {
                "123",
                "456",
                "789"
            };

            // ACT
            var result = GridUtils.RotateGrid(input, 0);

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    "123",
                    "456",
                    "789"
                },
                WithStrictOrdering);
        }

        [Test]
        public void RotateGrid_3x3_90deg()
        {
            var input = new[]
            {
                "123",
                "456",
                "789"
            };

            // ACT
            var result = GridUtils.RotateGrid(input, 90);

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    "741",
                    "852",
                    "963"
                },
                WithStrictOrdering);
        }

        [Test]
        public void RotateGrid_3x3_180deg()
        {
            var input = new[]
            {
                "123",
                "456",
                "789"
            };

            // ACT
            var result = GridUtils.RotateGrid(input, 180);

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    "987",
                    "654",
                    "321"
                },
                WithStrictOrdering);
        }

        [Test]
        public void RotateGrid_3x3_270deg()
        {
            var input = new[]
            {
                "123",
                "456",
                "789"
            };

            // ACT
            var result = GridUtils.RotateGrid(input, 270);

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    "369",
                    "258",
                    "147"
                },
                WithStrictOrdering);
        }

        [Test]
        public void RotateGrid_4x4_90deg()
        {
            var input = new[]
            {
                "   •",
                "  • ",
                " •• ",
                "••••"
            };

            // ACT
            var result = GridUtils.RotateGrid(input, 90);

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    "•   ",
                    "••  ",
                    "••• ",
                    "•  •"
                },
                WithStrictOrdering);
        }

        [Test]
        public void RotateGrid_SeaMonster_90deg()
        {
            var input = new[]
            {
                "                  # ",
                "#    ##    ##    ###",
                " #  #  #  #  #  #   "
            };

            // ACT
            var result = GridUtils.RotateGrid(input, 90);

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    " # ",
                    "#  ",
                    "   ",
                    "   ",
                    "#  ",
                    " # ",
                    " # ",
                    "#  ",
                    "   ",
                    "   ",
                    "#  ",
                    " # ",
                    " # ",
                    "#  ",
                    "   ",
                    "   ",
                    "#  ",
                    " # ",
                    " ##",
                    " # "
                },
                WithStrictOrdering);
        }
    }

    public class TheScaleGridMethod
    {
        [Test]
        public void ScaleGrid_SimpleTest()
        {
            var input = new[]
            {
                "12",
                "34"
            };

            // ACT
            var result = GridUtils.ScaleGrid(input, new Vector2(-1, 1));

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    "21",
                    "43"
                },
                WithStrictOrdering);
        }

        [Test]
        public void ScaleGrid_NoScaleTest()
        {
            var input = new[]
            {
                "12",
                "34"
            };

            // ACT
            var result = GridUtils.ScaleGrid(input, new Vector2(1, 1));

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    "12",
                    "34"
                },
                WithStrictOrdering);
        }

        [Test]
        public void ScaleGrid_SeaMonster_FlipY()
        {
            var input = new[]
            {
                "                  # ",
                "#    ##    ##    ###",
                " #  #  #  #  #  #   "
            };

            // ACT
            var result = GridUtils.ScaleGrid(input, new Vector2(1, -1));

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    " #  #  #  #  #  #   ",
                    "#    ##    ##    ###",
                    "                  # "
                },
                WithStrictOrdering);
        }

        [Test]
        public void ScaleGrid_SeaMonster_FlipX()
        {
            var input = new[]
            {
                "                  # ",
                "#    ##    ##    ###",
                " #  #  #  #  #  #   "
            };

            // ACT
            var result = GridUtils.ScaleGrid(input, new Vector2(-1, 1));

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    " #                  ",
                    "###    ##    ##    #",
                    "   #  #  #  #  #  # "
                },
                WithStrictOrdering);
        }

        [Test]
        public void ScaleGrid_SeaMonster_FlipXAndY()
        {
            var input = new[]
            {
                "                  # ",
                "#    ##    ##    ###",
                " #  #  #  #  #  #   "
            };

            // ACT
            var result = GridUtils.ScaleGrid(input, new Vector2(-1, -1));

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    "   #  #  #  #  #  # ",
                    "###    ##    ##    #",
                    " #                  "
                },
                WithStrictOrdering);
        }
    }

    public class ToStringGridMethod
    {
        [Test]
        public void ToStringGrid_DoesTranslateGridThatStartsWithNegativeBounds()
        {
            (Vector2 pos, char chr)[] input =
            {
                (new Vector2(-1, -1), 'A'),
                (new Vector2(1, 1), 'B')
            };

            // ACT
            var result = input.ToStringGrid(x => x.pos, x => x.chr, ' ');

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    "A  ",
                    "   ",
                    "  B"
                },
                WithStrictOrdering);
        }

        [Test]
        public void ToStringGrid_DoesTranslateGridThatStartsWithPositiveBounds()
        {
            (Vector2 pos, char chr)[] input =
            {
                (new Vector2(10, 10), 'A'),
                (new Vector2(12, 12), 'B')
            };

            // ACT
            var result = input.ToStringGrid(x => x.pos, x => x.chr, ' ');

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    "A  ",
                    "   ",
                    "  B"
                },
                WithStrictOrdering);
        }

        [Test]
        public void ToStringGrid_DoesIgnorePreviousDuplicateItemsInTheSamePosition()
        {
            (Vector2 pos, char chr)[] input =
            {
                (new Vector2(-1, -1), 'A'),
                (new Vector2(1, 1), 'B'),
                (new Vector2(1, 1), 'C')
            };

            // ACT
            var result = input.ToStringGrid(x => x.pos, x => x.chr, ' ');

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    "A  ",
                    "   ",
                    "  C"
                },
                WithStrictOrdering);
        }

        [Test]
        public void ToStringGrid_DoesCreateLargeGrid()
        {
            (Vector2 pos, char chr)[] input =
            {
                (new Vector2(1, 1), '0'),
                (new Vector2(1, 2), '1'),
                (new Vector2(5, 5), '5')
            };

            // ACT
            var result = input.ToStringGrid(x => x.pos, x => x.chr, '-');

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    "0----",
                    "1----",
                    "-----",
                    "-----",
                    "----5"
                },
                WithStrictOrdering);
        }

        [Test]
        public void ToStringGrid_DoesCreateRow()
        {
            (Vector2 pos, char chr)[] input =
            {
                (new Vector2(0, 0), '0'),
                (new Vector2(1, 0), '1'),
                (new Vector2(5, 0), '5')
            };

            // ACT
            var result = input.ToStringGrid(x => x.pos, x => x.chr, '-');

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    "01---5"
                },
                WithStrictOrdering);
        }

        [Test]
        public void ToStringGrid_DoesCreateColumn()
        {
            (Vector2 pos, char chr)[] input =
            {
                (new Vector2(0, 0), 'a'),
                (new Vector2(0, 1), 'b'),
                (new Vector2(0, 5), 'c')
            };

            // ACT
            var result = input.ToStringGrid(x => x.pos, x => x.chr, '#');

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    "a",
                    "b",
                    "#",
                    "#",
                    "#",
                    "c"
                },
                WithStrictOrdering);
        }
    }

    public class TheGetAdjacentMethod
    {
        [TestCase(-10, -10, "")]
        [TestCase(0, -10, "")]
        [TestCase(-10, 0, "")]
        [TestCase(0, 10, "")]
        [TestCase(10, 0, "")]
        [TestCase(10, 10, "")]
        [TestCase(1, -2, "")]
        [TestCase(1, 4, "")]

        [TestCase(-2, -1, "")]
        [TestCase(-1, -1, "1")]
        [TestCase(0, -1, "12")]
        [TestCase(1, -1, "123")]
        [TestCase(2, -1, "23")]
        [TestCase(3, -1, "3")]
        [TestCase(4, -1, "")]

        [TestCase(-2, 0, "")]
        [TestCase(-1, 0, "14")]
        [TestCase(0, 0, "245")]
        [TestCase(1, 0, "13456")]
        [TestCase(2, 0, "256")]
        [TestCase(3, 0, "36")]
        [TestCase(4, 0, "")]

        [TestCase(-2, 1, "")]
        [TestCase(-1, 1, "147")]
        [TestCase(0, 1, "12578")]
        [TestCase(1, 1, "12346789")]
        [TestCase(2, 1, "23589")]
        [TestCase(3, 1, "369")]
        [TestCase(4, 1, "")]

        [TestCase(-2, 2, "")]
        [TestCase(-1, 2, "47")]
        [TestCase(0, 2, "458")]
        [TestCase(1, 2, "45679")]
        [TestCase(2, 2, "568")]
        [TestCase(3, 2, "69")]
        [TestCase(4, 2, "")]

        [TestCase(-2, 3, "")]
        [TestCase(-1, 3, "7")]
        [TestCase(0, 3, "78")]
        [TestCase(1, 3, "789")]
        [TestCase(2, 3, "89")]
        [TestCase(3, 3, "9")]
        [TestCase(4, 3, "")]
        public void GetAdjacent_Tests(int posX, int posY, string expectedResultString)
        {
            const string gridData = "123 " +
                                    "456 " +
                                    "789";

            var grid = gridData.Split(" ")
                .Select((line, y) => line.Select((c, x) => new { Char = c, Position = new Vector2(x, y) }).ToArray())
                .ToArray();

            // ACT
            var results = grid.GetAdjacent(new Vector2(posX, posY));

            // ASSERT
            var resultString = string.Join("", results.Select(item => item.Char));

            resultString.Should().Be(expectedResultString);
        }
    }
}

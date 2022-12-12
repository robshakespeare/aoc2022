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

    public class TheToGridFromStringMethod
    {
        public record Cell(char Char, Vector2 Position);

        [Test]
        public void ToGrid_FromString_ReturnsExpectedResult()
        {
            const string input = """
                abc
                xyz
                """;

            // ACT
            var result = input.ToGrid((pos, chr) => new Cell(chr, pos));

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    new Cell[] {new('a', new(0, 0)), new('b', new(1, 0)), new('c', new(2, 0))},
                    new Cell[] {new('x', new(0, 1)), new('y', new(1, 1)), new('z', new(2, 1))}
                },
                opts => opts.WithStrictOrdering());
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
            const string gridData = """
                123
                456
                789
                """;

            var grid = gridData.ReadLines()
                .Select((line, y) => line.Select((c, x) => new { Char = c, Position = new Vector2(x, y) }).ToArray())
                .ToArray();

            // ACT
            var results = grid.GetAdjacent(new Vector2(posX, posY));

            // ASSERT
            var resultString = string.Join("", results.Select(item => item.Char));

            resultString.Should().Be(expectedResultString);
        }
    }

    public class TheRenderGridToConsoleMethod
    {
        [Test]
        public void RenderGridToConsole_Test()
        {
            var grid = """
                30373
                25512
                65332
                33549
                35390
                """.ReadLines().ToArray();

            // ACT
            var result = grid.RenderGridToConsole();

            Console.WriteLine("Hello world, this is for manual eyeballing");

            // ASSERT
            result.Should().Be("""
                30373
                25512
                65332
                33549
                35390
                """.ReplaceLineEndings());
        }
    }

    public class TheGetItemMethod
    {
        public record Item(char Char);

        private readonly Item[][] _grid;

        public TheGetItemMethod()
        {
            _grid = """
                abcd
                efgh
                """.ReadLines().Select(line => line.Select(c => new Item(c)).ToArray()).ToArray();
        }

        [TestCase(0, 0, 'a')]
        [TestCase(1, 0, 'b')]
        [TestCase(2, 0, 'c')]
        [TestCase(3, 0, 'd')]
        [TestCase(0, 1, 'e')]
        [TestCase(1, 1, 'f')]
        [TestCase(2, 1, 'g')]
        [TestCase(3, 1, 'h')]
        public void Get_ReturnsValueAsExpected(float x, float y, char expectedChar)
        {
            // ACT
            var actualChar = _grid.Get(new Vector2(x, y));

            // ASSERT
            actualChar.Should().BeEquivalentTo(new Item(expectedChar));
            actualChar.Should().Be(new Item(expectedChar));
        }

        [TestCase(-1, 0)]
        [TestCase(0, -1)]
        [TestCase(4, 0)]
        [TestCase(0, 2)]
        [TestCase(-99, -99)]
        [TestCase(99, 99)]
        [TestCase(-10, 10)]
        [TestCase(10, -10)]
        public void Get_ThrowsWhenExpected(float x, float y)
        {
            var action = () => _grid.Get(new Vector2(x, y));

            // ACT & ASSERT
            action.Should().Throw<Exception>().Which.Should().Match(e => e is IndexOutOfRangeException || e is ArgumentOutOfRangeException);
        }
    }

    public class TheGetCharacterMethod
    {
        [TestCase(0, 0, 'a')]
        [TestCase(1, 0, 'b')]
        [TestCase(2, 0, 'c')]
        [TestCase(3, 0, 'd')]
        [TestCase(0, 1, 'e')]
        [TestCase(1, 1, 'f')]
        [TestCase(2, 1, 'g')]
        [TestCase(3, 1, 'h')]
        public void Get_ReturnsValueAsExpected(float x, float y, char expectedChar)
        {
            var grid = """
                abcd
                efgh
                """.ReadLines().ToArray();

            // ACT
            var actualChar = grid.Get(new Vector2(x, y));

            // ASSERT
            actualChar.Should().Be(expectedChar);
        }

        [TestCase(-1, 0)]
        [TestCase(0, -1)]
        [TestCase(4, 0)]
        [TestCase(0, 2)]
        [TestCase(-99, -99)]
        [TestCase(99, 99)]
        [TestCase(-10, 10)]
        [TestCase(10, -10)]
        public void Get_ThrowsWhenExpected(float x, float y)
        {
            var grid = """
                abcd
                efgh
                """.ReadLines().ToArray();

            var action = () => grid.Get(new Vector2(x, y));

            // ACT & ASSERT
            action.Should().Throw<Exception>().Which.Should().Match(e => e is IndexOutOfRangeException || e is ArgumentOutOfRangeException);
        }
    }

    public class TheTryGetCharacterMethod
    {
        [TestCase(0, 0, true, 'a')]
        [TestCase(1, 0, true, 'b')]
        [TestCase(2, 0, true, 'c')]
        [TestCase(3, 0, true, 'd')]
        [TestCase(0, 1, true, 'e')]
        [TestCase(1, 1, true, 'f')]
        [TestCase(2, 1, true, 'g')]
        [TestCase(3, 1, true, 'h')]
        [TestCase(-1, 0, false, (char)0)]
        [TestCase(0, -1, false, (char)0)]
        [TestCase(4, 0, false, (char)0)]
        [TestCase(0, 2, false, (char)0)]
        [TestCase(-99, -99, false, (char)0)]
        [TestCase(99, 99, false, (char)0)]
        [TestCase(-10, 10, false, (char)0)]
        [TestCase(10, -10, false, (char)0)]
        public void TryGet_ReturnsValueAsExpected(float x, float y, bool expectedIsRetrieved, char expectedChar)
        {
            var grid = """
                abcd
                efgh
                """.ReadLines().ToArray();

            // ACT
            var actualIsRetrieved = grid.TryGet(new Vector2(x, y), out var actualChar);

            // ASSERT
            using (new AssertionScope())
            {
                actualIsRetrieved.Should().Be(expectedIsRetrieved);
                actualChar.Should().Be(expectedChar);
            }
        }
    }

    public class TheRenderWorldToViewportMethod
    {
        public record ExampleWorldItem(Vector2 Position, char Char);

        [Test]
        public void RenderWorldToViewport_DoesCenterWorldInViewport()
        {
            var input = new ExampleWorldItem[]
            {
                new(new(23, 23), 'M'),
                new(new(22, 21), 'A'),
                new(new(25, 24), 'B')
            };

            // ACT
            var result = input.RenderWorldToViewport(x => x.Position, x => x.Char, '.', viewportWidth: 8, viewportHeight: 6);

            Console.WriteLine(result);

            // ASSERT
            result.Should().Be("""
                ........
                ..A.....
                ........
                ...M....
                .....B..
                ........
                """.ReplaceLineEndings());
        }

        [Test]
        public void RenderWorldToViewport_CanAdditionallyRenderCenterOfWorld()
        {
            var input = new ExampleWorldItem[]
            {
                new(new(-1, -1), '*'),
                new(new(1, 1), '@')
            };

            // ACT
            var result = input.RenderWorldToViewport(x => x.Position, x => x.Char, '.', viewportWidth: 5, viewportHeight: 5, centerChar: 'X');

            Console.WriteLine(result);

            // ASSERT
            result.Should().Be("""
                .....
                .*...
                ..X..
                ...@.
                .....
                """.ReplaceLineEndings());
        }

        [Test]
        public void RenderWorldToViewport_DoesClipExtremitiesOfWorld()
        {
            var input = new ExampleWorldItem[]
            {
                new(new(-100, -100), '*'),
                new(new(0, 0), '['),
                new(new(1, 0), ']'),
                new(new(0, 1), '{'),
                new(new(1, 1), '}'),
                new(new(100, 100), '*'),
            };

            // ACT
            var result = input.RenderWorldToViewport(x => x.Position, x => x.Char, '_', viewportWidth: 4, viewportHeight: 4);

            Console.WriteLine(result);

            // ASSERT
            result.Should().Be("""
                ____
                _[]_
                _{}_
                ____
                """.ReplaceLineEndings());
        }
    }
}

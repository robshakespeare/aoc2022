using AoC.Day00;

namespace AoC.Tests.Day00;

public class Day0SolverTests
{
    private readonly Day0Solver _sut = new()
    {
        SimulateSlowness = false
    };

    [Test]
    public void DataAccessors_Should_ReturnValuesAsExpected()
    {
        _sut.DayName.Should().Be("Test Day");
        _sut.GetDayNumber().Should().Be(0);
        _sut.GetTitle().Should().Contain("Day 0").And.Contain("Test Day");
    }

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
        _sut.SolvePart1(out var part1ResultFull);

        // ASSERT
        part1Result.Should().Be(136);

        part1ResultFull.Value.Should().Be(136);
        part1ResultFull.IsStarted.Should().BeTrue();
        part1ResultFull.IsCompleted.Should().BeTrue();
    }

    [Test]
    public void Part2Example()
    {
        // ACT
        var part2ExampleResult = _sut.SolvePart2("8,18,24");

        // ASSERT
        part2ExampleResult.Should().EndWith("72");
    }

    [Test]
    public void Part2ReTest()
    {
        // ACT
        var part2Result = _sut.SolvePart2();
        _sut.SolvePart2(out var part2ResultFull);

        // ASSERT
        part2Result.Should().EndWith("840").And.StartWith("Hello World!");

        part2ResultFull.Value.Should().BeOfType<string>().Which.Should().EndWith("840");
        part2ResultFull.IsStarted.Should().BeTrue();
        part2ResultFull.IsCompleted.Should().BeTrue();
    }

    [Test]
    public async Task RunAsync_Flow_Test()
    {
        var actualResults = new List<Results>();

        // ACT
        await _sut.RunAsync(async results =>
        {
            actualResults.Add(results);
            await Task.Delay(10);
        });

        // ASSERT
        actualResults.Should().BeEquivalentTo(
            new[]
            {
                new
                {
                    Part1Result = new
                    {
                        IsStarted = true,
                        IsCompleted = false,
                        IsRunning = true,
                        IsCompletedWithValue = false,
                        IsCompletedWithoutValue = false
                    },
                    Part2Result = new
                    {
                        IsStarted = false,
                        IsCompleted = false,
                        IsRunning = false,
                        IsCompletedWithValue = false,
                        IsCompletedWithoutValue = false
                    }
                },
                new
                {
                    Part1Result = new
                    {
                        IsStarted = true,
                        IsCompleted = true,
                        IsRunning = false,
                        IsCompletedWithValue = true,
                        IsCompletedWithoutValue = false
                    },
                    Part2Result = new
                    {
                        IsStarted = true,
                        IsCompleted = false,
                        IsRunning = true,
                        IsCompletedWithValue = false,
                        IsCompletedWithoutValue = false
                    }
                },
                new
                {
                    Part1Result = new
                    {
                        IsStarted = true,
                        IsCompleted = true,
                        IsRunning = false,
                        IsCompletedWithValue = true,
                        IsCompletedWithoutValue = false
                    },
                    Part2Result = new
                    {
                        IsStarted = true,
                        IsCompleted = true,
                        IsRunning = false,
                        IsCompletedWithValue = true,
                        IsCompletedWithoutValue = false
                    }
                }
            },
            opts => opts.WithStrictOrdering());

        // Assert value flow
        actualResults[0].Part1Result.Value.Should().Be(null);
        actualResults[0].Part2Result.Value.Should().Be(null);

        actualResults[1].Part1Result.Value.Should().Be(136L);
        actualResults[1].Part2Result.Value.Should().Be(null);

        actualResults[2].Part1Result.Value.Should().Be(136L);
        actualResults[2].Part2Result.Value.Should().BeOfType<string>().Which.Should().EndWith("840").And.StartWith("Hello World!");

        // Assert time taken flow
        actualResults[0].Part1Result.Elapsed.Should().Be(null);
        actualResults[0].Part2Result.Elapsed.Should().Be(null);
        

        actualResults[1].Part1Result.Elapsed.Should().BeGreaterThan(TimeSpan.Zero);
        actualResults[1].Part2Result.Elapsed.Should().Be(null);

        actualResults[2].Part1Result.Elapsed.Should().BeGreaterThan(TimeSpan.Zero);
        actualResults[2].Part2Result.Elapsed.Should().BeGreaterThan(TimeSpan.Zero);

        // Assert time taken (seconds) flow
        actualResults[0].Part1Result.ElapsedTotalSeconds.Should().Be(0);
        actualResults[0].Part2Result.ElapsedTotalSeconds.Should().Be(0);

        actualResults[1].Part1Result.ElapsedTotalSeconds.Should().BeGreaterThan(0);
        actualResults[1].Part2Result.ElapsedTotalSeconds.Should().Be(0);

        actualResults[2].Part1Result.ElapsedTotalSeconds.Should().BeGreaterThan(0);
        actualResults[2].Part2Result.ElapsedTotalSeconds.Should().BeGreaterThan(0);

        // Complete state
        actualResults[2].Part1Result.IsRunning.Should().BeFalse();
        actualResults[2].Part1Result.IsCompletedWithValue.Should().BeTrue();
        actualResults[2].Part1Result.IsCompletedWithoutValue.Should().BeFalse();

        actualResults[2].Part2Result.IsRunning.Should().BeFalse();
        actualResults[2].Part2Result.IsCompletedWithValue.Should().BeTrue();
        actualResults[2].Part2Result.IsCompletedWithoutValue.Should().BeFalse();
    }
}

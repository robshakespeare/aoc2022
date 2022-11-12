using System.Diagnostics;
using static Crayon.Output;

namespace AoC;

public class TimingBlock : IDisposable
{
    private readonly string _name;
    private readonly Stopwatch _stopwatch;

    public TimingBlock(string name)
    {
        _name = name;
        _stopwatch = Stopwatch.StartNew();
    }

    public TimeSpan Stop()
    {
        _stopwatch.Stop();
        return _stopwatch.Elapsed;
    }

    public void Dispose()
    {
        Stop();

        Console.WriteLine(Rgb(118, 118, 118).Text($"[{_name}] time taken (seconds): {_stopwatch.Elapsed.TotalSeconds:0.000000}"));
        Console.WriteLine();
    }
}

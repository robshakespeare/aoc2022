using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AoC.MAUI;

public class SolverViewModel : INotifyPropertyChanged
{
    public SolverViewModel(ISolver solver)
    {
        Title = solver.Title;
    }

    private Result _part1Result;
    private Result _part2Result;
    private bool _isComplete;

    public string Title { get; }

    public Result Part1Result
    {
        get => _part1Result;
        set => SetField(ref _part1Result, value);
    }

    public Result Part2Result
    {
        get => _part2Result;
        set => SetField(ref _part2Result, value);
    }

    public bool IsComplete
    {
        get => _isComplete;
        set => SetField(ref _isComplete, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

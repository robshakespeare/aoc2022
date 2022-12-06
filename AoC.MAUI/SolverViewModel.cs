using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AoC.MAUI;

public class SolverViewModel : INotifyPropertyChanged
{
    public SolverViewModel(ISolverBase solver)
    {
        Title = solver.GetTitle();
    }

    private Results _results;
    private bool _isComplete;

    public string Title { get; }

    public Results Results
    {
        get => _results;
        set => SetField(ref _results, value);
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
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

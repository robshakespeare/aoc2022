using System.Windows.Input;

namespace AoC.MAUI;

public partial class MainPage
{
    private Task? _currentSolverTask;

    public MainPage()
    {
        RunDay(SolverFactory.DefaultDay);
        InitializeComponent();
    }

    public SolverViewModel ViewModel => (SolverViewModel) BindingContext;

    private void RunDay(string dayNumber)
    {
        if (_currentSolverTask is {IsCompleted: false})
        {
            return; // Don't start a new solver until the current one has finished!
        }

        _currentSolverTask?.Dispose();

        var solver = SolverFactory.Instance.TryCreateSolver(dayNumber) ?? throw new InvalidOperationException("No solver found for day " + dayNumber);
        var solverViewModel = new SolverViewModel(solver);

        BindingContext = solverViewModel;
        OnPropertyChanged(nameof(ViewModel));

        _currentSolverTask = Task.Run(async () =>
        {
            await solver.RunAsync(() =>
            {
                solverViewModel.Part1Result = solver.Part1Result;
                solverViewModel.Part2Result = solver.Part2Result;
                return Task.CompletedTask;
            });
            solverViewModel.IsComplete = true;
        });
    }

    public ICommand RunDayCommand => new Command<int>(dayNumber => RunDay(dayNumber.ToString()));
}

namespace AoC.MAUI;

public partial class MainPage
{
    private readonly ISolverFactory _solverFactory;
    private readonly IDictionary<string, string> _dayTitlesToNumbers;

    public MainPage(ISolverFactory solverFactory)
    {
        _solverFactory = solverFactory;
        _dayTitlesToNumbers = _solverFactory.Solvers.ToDictionary(
            x => string.IsNullOrEmpty(x.DayName) ? x.DayNumber : $"{x.DayNumber}: {x.DayName}",
            x => x.DayNumber);

        RunDay(solverFactory.DefaultDay);
        InitializeComponent();
    }

    public SolverViewModel ViewModel => (SolverViewModel) BindingContext;

    private void RunDay(string dayNumber)
    {
        var solver = _solverFactory.CreateSolver(dayNumber);
        var solverViewModel = new SolverViewModel(solver);

        BindingContext = solverViewModel;
        OnPropertyChanged(nameof(ViewModel));

        Task.Run(async () =>
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

    private async void ChooseDayButtonClickedAsync(object? sender, EventArgs e)
    {
        const string cancel = "Cancel";
        var dayTitle = await DisplayActionSheet("Choose Day", cancel, null, _dayTitlesToNumbers.Keys.ToArray());

        if (dayTitle != cancel)
        {
            RunDay(_dayTitlesToNumbers[dayTitle]);
        }
    }
}

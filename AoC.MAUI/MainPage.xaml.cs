namespace AoC.MAUI;

public partial class MainPage
{
    public MainPage()
    {
        RunDay(SolverFactory.DefaultDay);
        InitializeComponent();
    }

    public SolverViewModel ViewModel => (SolverViewModel) BindingContext;

    private void RunDay(string dayNumber)
    {
        var solver = SolverFactory.Instance.TryCreateSolver(dayNumber) ?? throw new InvalidOperationException("No solver found for day " + dayNumber);
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
        var day = await DisplayActionSheet("Choose Day", cancel, null, SolverFactory.Days.Select(day => day.ToString()).ToArray());

        if (day != cancel)
        {
            RunDay(day);
        }
    }
}

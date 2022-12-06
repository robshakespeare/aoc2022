namespace AoC.MAUI;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new MainPage(SolverFactory.Instance);
    }
}

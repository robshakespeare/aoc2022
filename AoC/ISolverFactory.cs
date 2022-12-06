namespace AoC;

public interface ISolverFactory
{
    IReadOnlyCollection<(string DayNumber, string DayName)> Solvers { get; }

    string DefaultDay { get; }

    ISolverBase? TryCreateSolver(string? dayNumber);

    ISolverBase CreateSolver(string? dayNumber);
}

namespace AoC;

public interface ISolverFactory
{
    IReadOnlyCollection<(string DayNumber, string DayName)> Solvers { get; }

    string DefaultDay { get; }

    ISolver? TryCreateSolver(string? dayNumber);

    ISolver CreateSolver(string? dayNumber);
}
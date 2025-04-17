namespace Sudoku.Solvers
{
    /// <summary>
    /// Represents an algorithm used for solving a sudoku grid.
    /// </summary>
    public interface ISolvingAlgorithm
    {
        /// <summary>
        /// Solves the provided <see cref="Grid"/> if possible. If not, false will be returned.
        /// </summary>
        /// <param name="grid">The grid to try to solve.</param>
        /// <returns>A boolean value indicating wether or not the grid could be solved.</returns>
        bool SolveGrid(Grid grid);
    }
}
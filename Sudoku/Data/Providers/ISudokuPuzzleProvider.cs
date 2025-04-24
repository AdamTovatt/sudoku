using Sudoku.Data.Models;

namespace Sudoku.Data.Providers
{
    /// <summary>
    /// For getting sudoku puzzles.
    /// </summary>
    public interface ISudokuPuzzleProvider : IDisposable
    {
        /// <summary>
        /// Will get the next puzzle, optionally with a specific difficulty, otherwise just the next regardless of difficulty.
        /// </summary>
        /// <param name="difficulty">Optional parameter for specifying difficulty of the puzzle to get. Default is Unspecified which means it will just return the next puzzle regardless of difficulty.</param>
        public SudokuPuzzle? GetNext(PuzzleDifficulty difficulty = PuzzleDifficulty.Unspecified);
    }
}

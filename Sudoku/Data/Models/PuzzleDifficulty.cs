namespace Sudoku.Data.Models
{
    /// <summary>
    /// Represents the difficulty level of a Sudoku puzzle.
    /// </summary>
    public enum PuzzleDifficulty
    {
        /// <summary>
        /// Difficulty not specified.
        /// </summary>
        Unspecified,

        /// <summary>
        /// Easy difficulty (0.0 ≤ value &lt; 1.0).
        /// </summary>
        Easy,

        /// <summary>
        /// Medium difficulty (1.0 ≤ value &lt; 2.0).
        /// </summary>
        Medium,

        /// <summary>
        /// Hard difficulty (2.0 ≤ value &lt; 3.0).
        /// </summary>
        Hard,

        /// <summary>
        /// Expert difficulty (value ≥ 3.0).
        /// </summary>
        Expert
    }
}

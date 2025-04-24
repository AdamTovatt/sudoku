using System.Globalization;
using System.Text;

namespace Sudoku.Data.Models
{
    /// <summary>
    /// Represents a Sudoku puzzle with its starting grid, solution, difficulty, and clue count.
    /// </summary>
    public class SudokuPuzzle
    {
        /// <summary>
        /// Gets or sets the grid representing the unsolved puzzle.
        /// </summary>
        public Grid StartingGrid { get; set; }

        /// <summary>
        /// Gets or sets the grid representing the solved puzzle.
        /// </summary>
        public Grid SolvedGrid { get; set; }

        /// <summary>
        /// Gets or sets the raw numeric difficulty value of the puzzle.
        /// </summary>
        public double DifficultyValue { get; set; }

        /// <summary>
        /// Gets or sets the number of given clues in the puzzle.
        /// </summary>
        public int Clues { get; set; }

        /// <summary>
        /// Gets the interpreted difficulty category based on <see cref="DifficultyValue"/>.
        /// </summary>
        public PuzzleDifficulty Difficulty => DifficultyValue switch
        {
            >= 0.0 and < 1.0 => PuzzleDifficulty.Easy,
            >= 1.0 and < 2.0 => PuzzleDifficulty.Medium,
            >= 2.0 and < 3.0 => PuzzleDifficulty.Hard,
            _ => PuzzleDifficulty.Expert
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="SudokuPuzzle"/> class.
        /// </summary>
        /// <param name="startingGrid">The unsolved puzzle grid.</param>
        /// <param name="solvedGrid">The solved puzzle grid.</param>
        /// <param name="difficultyValue">The raw difficulty value of the puzzle.</param>
        /// <param name="clues">The number of clues given in the puzzle.</param>
        public SudokuPuzzle(Grid startingGrid, Grid solvedGrid, double difficultyValue, int clues)
        {
            StartingGrid = startingGrid;
            SolvedGrid = solvedGrid;
            DifficultyValue = difficultyValue;
            Clues = clues;
        }

        /// <summary>
        /// Creates a <see cref="SudokuPuzzle"/> from a CSV-formatted string.
        /// </summary>
        /// <param name="line">A CSV line in the format: id,puzzle,solution,clues,difficulty</param>
        /// <returns>A new <see cref="SudokuPuzzle"/> instance or null if parsing fails.</returns>
        public static SudokuPuzzle? FromString(string line)
        {
            string[] parts = line.Split(',');

            if (parts.Length < 5)
                return null;

            try
            {
                Grid puzzleGrid = Grid.CreateFromString(parts[1]);
                Grid solutionGrid = Grid.CreateFromString(parts[2]);
                int clues = int.Parse(parts[3]);
                double difficultyValue = double.Parse(parts[4], CultureInfo.InvariantCulture);

                return new SudokuPuzzle(puzzleGrid, solutionGrid, difficultyValue, clues);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the CSV header string for a <see cref="SudokuPuzzle"/>.
        /// </summary>
        /// <returns>A string representing the CSV header: "id,puzzle,solution,clues,difficulty"</returns>
        public static string GetCsvHeader()
        {
            return "id,puzzle,solution,clues,difficulty";
        }

        /// <summary>
        /// Returns a CSV-formatted string representing the puzzle.
        /// </summary>
        /// <returns>A string in the format: id,puzzle,solution,clues,difficulty</returns>
        public override string ToString()
        {
            string puzzle = StartingGrid.ToString().Replace(" ", "").Replace("\n", "");
            string solution = SolvedGrid.ToString().Replace(" ", "").Replace("\n", "");

            return $"0,{puzzle},{solution},{Clues},{DifficultyValue.ToString(CultureInfo.InvariantCulture)}";
        }
    }
}
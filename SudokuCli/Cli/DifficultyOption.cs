using Sudoku.Data.Models;

namespace SudokuCli.Cli
{
    public class DifficultyOption
    {
        public string Name { get; }

        public DifficultyOption(string name)
        {
            Name = name;
        }

        public PuzzleDifficulty? GetDifficulty()
        {
            return Enum.TryParse<PuzzleDifficulty>(Name, true, out PuzzleDifficulty parsed)
                ? parsed
                : null;
        }
    }
}

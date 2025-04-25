using Sudoku.Data.Models;
using Sudoku.Solvers;

namespace SudokuCli.Cli
{
    public class InputArguments
    {
        public ISolvingAlgorithm SolvingAgorithm { get; set; }
        public int Count { get; set; }
        public string OutputBasePath { get; set; }
        public PuzzleDifficulty Difficulty { get; set; }

        public InputArguments(ISolvingAlgorithm solvingAgorithm, int count, string outputBasePath, PuzzleDifficulty difficulty)
        {
            SolvingAgorithm = solvingAgorithm;
            Count = count;
            OutputBasePath = outputBasePath;
            Difficulty = difficulty;
        }
    }
}

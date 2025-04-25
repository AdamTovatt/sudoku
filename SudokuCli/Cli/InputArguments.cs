using Sudoku.Solvers;

namespace SudokuCli.Cli
{
    public class InputArguments
    {
        public ISolvingAlgorithm SolvingAgorithm { get; set; }
        public int Count { get; set; }
        public string OutputBasePath { get; set; }

        public InputArguments(ISolvingAlgorithm solvingAgorithm, int count, string outputBasePath)
        {
            SolvingAgorithm = solvingAgorithm;
            Count = count;
            OutputBasePath = outputBasePath;
        }
    }
}

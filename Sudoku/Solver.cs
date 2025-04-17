using Sudoku.Solvers;

namespace Sudoku
{
    public class Solver
    {
        public static ISolvingAlgorithm BruteForceAlgorithm { get; private set; }
        public static ISolvingAlgorithm BitAlgorithm { get; private set; }

        static Solver()
        {
            BruteForceAlgorithm = new BruteForceAlgorithm();
            BitAlgorithm = new BitAlgorithm();
        }

        public static bool SolveWith(Grid gridToSolve, ISolvingAlgorithm algorithmToSolveWith)
        {
            return algorithmToSolveWith.SolveGrid(gridToSolve);
        }
    }
}

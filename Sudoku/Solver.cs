using Sudoku.Solvers;

namespace Sudoku
{
    public class Solver
    {
        public static ISolvingAlgorithm BruteForceAlgorithm { get; private set; }
        public static ISolvingAlgorithm MVRAlgorithm { get; private set; }
        public static ISolvingAlgorithm MVRAlgorithm2 { get; private set; }

        static Solver()
        {
            BruteForceAlgorithm = new BruteForceAlgorithm();
            MVRAlgorithm = new MVRAlgorithm();
            MVRAlgorithm2 = new MVRAlgorithm2();
        }

        public static bool SolveWith(Grid gridToSolve, ISolvingAlgorithm algorithmToSolveWith)
        {
            return algorithmToSolveWith.SolveGrid(gridToSolve);
        }
    }
}

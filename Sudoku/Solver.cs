using Sudoku.Solvers;

namespace Sudoku
{
    public class Solver
    {
        public static ISolvingAlgorithm BruteForceAlgorithm { get; private set; }
        public static ISolvingAlgorithm BitAlgorithm { get; private set; }
        public static ISolvingAlgorithm MVRAlgorithm { get; private set; }
        public static ISolvingAlgorithm MVRAlgorithm2 { get; private set; }
        public static ISolvingAlgorithm LoadAssociatedAlgorithm { get; private set; }
        public static ISolvingAlgorithm PreprocessAlgorithm { get; private set; }

        static Solver()
        {
            BruteForceAlgorithm = new BruteForceAlgorithm();
            BitAlgorithm = new BitAlgorithm();
            MVRAlgorithm = new MVRAlgorithm();
            MVRAlgorithm2 = new MVRAlgorithm2();
            LoadAssociatedAlgorithm = new LoadAssociatedAlgorithm();
            PreprocessAlgorithm = new PreprocessAlgorithm();
        }

        public static bool SolveWith(Grid gridToSolve, ISolvingAlgorithm algorithmToSolveWith)
        {
            return algorithmToSolveWith.SolveGrid(gridToSolve);
        }
    }
}

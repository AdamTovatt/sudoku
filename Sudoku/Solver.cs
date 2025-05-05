using Sudoku.Solvers;
using System.Reflection;

namespace Sudoku
{
    /// <summary>
    /// Provides access to available Sudoku solving algorithms and a method to solve a grid using a chosen algorithm.
    /// </summary>
    public class Solver
    {
        /// <summary>
        /// Gets the brute-force solving algorithm.
        /// </summary>
        public static ISolvingAlgorithm BruteForceAlgorithm { get; private set; }

        /// <summary>
        /// Gets the MVR (Minimum Remaining Values) solving algorithm.
        /// </summary>
        public static ISolvingAlgorithm MVRAlgorithm { get; private set; }

        /// <summary>
        /// Gets an alternative implementation of the MVR solving algorithm.
        /// </summary>
        public static ISolvingAlgorithm MVRAlgorithm2 { get; private set; }

        /// <summary>
        /// Gets an algorithm that uses Rule 0 to eliminate digits.
        /// </summary>
        public static ISolvingAlgorithm PreprocessAlgorithm { get; private set; }

        /// <summary>
        /// Static constructor to initialize the solver algorithm instances.
        /// </summary>
        static Solver()
        {
            BruteForceAlgorithm = new BruteForceAlgorithm();
            MVRAlgorithm = new MVRAlgorithm();
            MVRAlgorithm2 = new MVRAlgorithm2();
            PreprocessAlgorithm = new PreprocessAlgorithm();
        }

        /// <summary>
        /// Solves the provided Sudoku grid using the specified solving algorithm.
        /// </summary>
        /// <param name="gridToSolve">The Sudoku grid to be solved.</param>
        /// <param name="algorithmToSolveWith">The solving algorithm to use.</param>
        /// <returns>True if the grid was successfully solved; otherwise, false.</returns>
        public static bool Solve(Grid gridToSolve, ISolvingAlgorithm algorithmToSolveWith)
        {
            return algorithmToSolveWith.SolveGrid(gridToSolve);
        }

        /// <summary>
        /// Gets all available solver types that implement the ISolvingAlgorithm interface.
        /// </summary>
        /// <returns>A list of solver types available in the current application domain.</returns>
        public static List<Type> GetAvailableSolvers()
        {
            Type interfaceType = typeof(ISolvingAlgorithm);
            List<Type> solverTypes = new List<Type>();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (interfaceType.IsAssignableFrom(type) &&
                        type.IsClass &&
                        !type.IsAbstract &&
                        type.IsPublic)
                    {
                        solverTypes.Add(type);
                    }
                }
            }

            return solverTypes;
        }
    }
}

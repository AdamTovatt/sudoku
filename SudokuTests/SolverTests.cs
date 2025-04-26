using Sudoku;
using Sudoku.Data.Models;
using Sudoku.Data.Providers;
using Sudoku.Solvers;

namespace SudokuTests
{
    [TestClass]
    public class SolverTests
    {
        [DataTestMethod]
        [DataRow(typeof(BruteForceAlgorithm))]
        [DataRow(typeof(MVRAlgorithm))]
        [DataRow(typeof(MVRAlgorithm2))]
        public void SolveWith(Type typeOfAlgorithm)
        {
            ISolvingAlgorithm solvingAlgorithm = CreateAlgorithmInstance(typeOfAlgorithm);

            using (ISudokuPuzzleProvider puzzleProvider = EmbeddedResourcesCsvStreamPuzzleProvider.Create())
            {
                AssertSolveForMultiplePuzzles(
                    puzzleCount: 10,
                    puzzleProvider: puzzleProvider,
                    solvingAlgorithm: solvingAlgorithm,
                    puzzleDifficulty: PuzzleDifficulty.Easy);

                AssertSolveForMultiplePuzzles(
                    puzzleCount: 10,
                    puzzleProvider: puzzleProvider,
                    solvingAlgorithm: solvingAlgorithm,
                    puzzleDifficulty: PuzzleDifficulty.Medium);

                AssertSolveForMultiplePuzzles(
                    puzzleCount: 10,
                    puzzleProvider: puzzleProvider,
                    solvingAlgorithm: solvingAlgorithm,
                    puzzleDifficulty: PuzzleDifficulty.Hard);

                AssertSolveForMultiplePuzzles(
                    puzzleCount: 10,
                    puzzleProvider: puzzleProvider,
                    solvingAlgorithm: solvingAlgorithm,
                    puzzleDifficulty: PuzzleDifficulty.Expert);

                AssertSolveForMultiplePuzzles(
                    puzzleCount: 10,
                    puzzleProvider: puzzleProvider,
                    solvingAlgorithm: solvingAlgorithm,
                    puzzleDifficulty: PuzzleDifficulty.Unspecified);
            }
        }

        private void AssertSolveForMultiplePuzzles(int puzzleCount, ISudokuPuzzleProvider puzzleProvider, ISolvingAlgorithm solvingAlgorithm, PuzzleDifficulty puzzleDifficulty)
        {
            for (int i = 0; i < puzzleCount; i++)
            {
                SudokuPuzzle? puzzle = puzzleProvider.GetNext(puzzleDifficulty);

                Assert.IsNotNull(puzzle);
                AssertSolveWorks(solvingAlgorithm, puzzle, i);
            }
        }

        private void AssertSolveWorks(ISolvingAlgorithm solvingAlgorithm, SudokuPuzzle puzzle, int index)
        {
            Grid startingGrid = puzzle.StartingGrid;
            bool didSolve = solvingAlgorithm.SolveGrid(startingGrid);
            bool isSolved = startingGrid.IsSolved(out InvalidCellInformation? invalidCellInformation);

            Assert.IsTrue(didSolve, $"Could not solve with \"{solvingAlgorithm.GetType()}\" for puzzle with difficulty {puzzle.DifficultyValue} ({puzzle.DifficultyValue}) at index {index}:\n{invalidCellInformation?.ToString()}\n{startingGrid}");
            Assert.IsTrue(isSolved);
            Assert.IsNull(invalidCellInformation);

            Assert.IsTrue(startingGrid.HasSameCellValuesAs(puzzle.SolvedGrid));
        }

        private ISolvingAlgorithm CreateAlgorithmInstance(Type type)
        {
            if (!typeof(ISolvingAlgorithm).IsAssignableFrom(type))
                throw new ArgumentException("Type must implement ISolvingAlgorithm", nameof(type));

            return (ISolvingAlgorithm)Activator.CreateInstance(type)!;
        }

        [TestMethod]
        public void GetAvailableSolvers()
        {
            List<Type> solverTypes = Solver.GetAvailableSolvers();

            foreach (Type type in solverTypes)
            {
                Assert.IsTrue(typeof(ISolvingAlgorithm).IsAssignableFrom(type));
                Assert.IsFalse(type.IsAbstract);
                Assert.IsTrue(type.IsClass);
                Assert.IsTrue(type.IsPublic);
            }

            // Optional: assert that known solvers are included
            CollectionAssert.Contains(solverTypes, typeof(BruteForceAlgorithm));
            CollectionAssert.Contains(solverTypes, typeof(MVRAlgorithm));
            CollectionAssert.Contains(solverTypes, typeof(MVRAlgorithm2));
        }
    }
}

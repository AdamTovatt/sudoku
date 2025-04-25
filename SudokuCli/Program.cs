using Sudoku;
using Sudoku.Data.Models;
using Sudoku.Data.Providers;
using Sudoku.Resources;
using Sudoku.Solvers;
using SudokuCli.Cli;
using SudokuCli.Exporting;
using System.Diagnostics;
using System.Reflection;

namespace SudokuCli
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ResourceHelper.Initialize(Assembly.GetAssembly(typeof(Solver))); // initialize resource helper so it can load embedded resources

            bool directLaunch = args.Length == 0;
            if (directLaunch) args = Console.ReadLine()!.Split(" ");

            InputArguments? arguments = ArgumentHandler.GetInputArguments(args);

            if (arguments == null)
            {
                Console.WriteLine("\nInvalid input resulted in early exit of the process.");

                if (directLaunch)
                {
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                }

                return;
            }

            EmbeddedResourcesCsvStreamPuzzleProvider puzzleProvider = EmbeddedResourcesCsvStreamPuzzleProvider.Create();

            List<SudokuPuzzle> puzzles = new List<SudokuPuzzle>();
            for (int i = 0; i < arguments.Count; i++)
            {
                SudokuPuzzle? puzzle = puzzleProvider.GetNext();

                if (puzzle == null)
                    throw new InvalidDataException($"Encountered unexpected end of puzzle stream");

                puzzles.Add(puzzle);
            }

            ISolvingAlgorithm solvingAlgorithm = arguments.SolvingAgorithm;

            List<DataTableHeaderCell> headerCells = new List<DataTableHeaderCell>()
            {
                new DataTableHeaderCell("index", typeof(int)),
                new DataTableHeaderCell("elapsed time (ms)", typeof(long)),
                new DataTableHeaderCell("memory used (bytes)", typeof(long)),
            };

            DataTable dataTable = new DataTable(headerCells);

            for (int i = 0; i < arguments.Count; i++)
            {
                dataTable.AddRow(MeasureSolveResourceUsage(i, solvingAlgorithm, puzzles[i]));
                Console.WriteLine("Solved: " + i);
            }

            File.WriteAllText(Path.Combine(arguments.OutputBasePath, $"{solvingAlgorithm.GetType().Name}-{arguments.Count}.csv"), dataTable.ToCsv());

            if (directLaunch)
            {
                Console.WriteLine("Done, press any key to exit...");
                Console.ReadKey();
            }
        }

        private static DataTableRow MeasureSolveResourceUsage(int index, ISolvingAlgorithm solvingAlgorithm, SudokuPuzzle puzzle)
        {
            Grid startingGrid = puzzle.StartingGrid;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            long beforeMemory = GC.GetTotalMemory(false);

            Stopwatch stopwatch = Stopwatch.StartNew();
            solvingAlgorithm.SolveGrid(startingGrid);
            stopwatch.Stop();

            long afterMemory = GC.GetTotalMemory(false);
            long memoryUsed = afterMemory - beforeMemory;

            List<object?> values = new List<object?>()
            {
                index,
                stopwatch.ElapsedMilliseconds,
                memoryUsed
            };

            return new DataTableRow(values);
        }
    }
}

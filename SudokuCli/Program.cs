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
            ResourceHelper.Initialize(Assembly.GetAssembly(typeof(Solver)));

            bool directLaunch = args.Length == 0;
            if (directLaunch) args = Console.ReadLine()!.Split(" ");

            InputArguments? arguments = ArgumentHandler.GetInputArguments(args);

            if (arguments == null)
            {
                Console.WriteLine("\nInvalid input resulted in early exit of the process.");
                if (directLaunch) Console.ReadKey();
                return;
            }

            // Preload ALL puzzles into memory at startup
            var puzzleProvider = PreloadAllPuzzlesIntoMemory();

            List<SudokuPuzzle> puzzles = new List<SudokuPuzzle>();
            for (int i = 0; i < arguments.Count; i++)
            {
                SudokuPuzzle? puzzle = puzzleProvider.GetNext(arguments.Difficulty);
                if (puzzle == null) break; // Exit if we've exhausted puzzles
                puzzles.Add(puzzle);
            }

            ISolvingAlgorithm solvingAlgorithm = arguments.SolvingAgorithm;

            List<DataTableHeaderCell> headerCells = new List<DataTableHeaderCell>()
            {
                new DataTableHeaderCell("index", typeof(int)),
                new DataTableHeaderCell("difficulty", typeof(PuzzleDifficulty)),
                new DataTableHeaderCell("elapsed time (ns)", typeof(double)),
                new DataTableHeaderCell("memory used (bytes)", typeof(long)),
            };

            DataTable dataTable = new DataTable(headerCells);

            for (int i = 0; i < arguments.Count; i++)
            {
                dataTable.AddRow(MeasureSolveResourceUsage(i, solvingAlgorithm, puzzles[i]));
                //Console.WriteLine("Solved: " + i);
            }

            File.WriteAllText(Path.Combine(arguments.OutputBasePath, $"{solvingAlgorithm.GetType().Name}-{arguments.Difficulty}-{arguments.Count}.csv"), dataTable.ToCsv());

            if (directLaunch)
            {
                Console.WriteLine("Done, press any key to exit...");
                Console.ReadKey();
            }
        }

        private static InMemoryPuzzleProvider PreloadAllPuzzlesIntoMemory()
        {
            var provider = new InMemoryPuzzleProvider();

            // Load puzzles from all difficulty streams
            PreloadDifficulty(provider, Resource.SudokuPuzzles.Easy);
            PreloadDifficulty(provider, Resource.SudokuPuzzles.Medium);
            PreloadDifficulty(provider, Resource.SudokuPuzzles.Hard);
            PreloadDifficulty(provider, Resource.SudokuPuzzles.Expert);

            return provider;
        }

        private static void PreloadDifficulty(InMemoryPuzzleProvider provider, Resource resource)
        {
            using var stream = ResourceHelper.Instance.GetFileStream(resource);
            using var csvProvider = CsvStreamPuzzleProvider.CreateWithStream(stream);

            SudokuPuzzle? puzzle;
            while ((puzzle = csvProvider.GetNext()) != null)
            {
                provider.AddPuzzle(puzzle);
            }
        }
        private static DataTableRow MeasureSolveResourceUsage(int index, ISolvingAlgorithm solvingAlgorithm, SudokuPuzzle puzzle)
        {
            Grid startingGrid = puzzle.StartingGrid;

            // Removed GC calls
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
            //GC.Collect();

            //long beforeMemory = GC.GetTotalMemory(false);

            Stopwatch stopwatch = Stopwatch.StartNew();
            solvingAlgorithm.SolveGrid(startingGrid);
            stopwatch.Stop();

            //long afterMemory = GC.GetTotalMemory(false);
            long memoryUsed = 0;

            List<object?> values = new List<object?>()
            {
                index,
                puzzle.Difficulty,
                stopwatch.Elapsed.TotalNanoseconds,
                memoryUsed
            };

            return new DataTableRow(values);
        }
    }
}

using System;
using System.Diagnostics;

using Sudoku;
using Sudoku.Solvers;

class Program
{
    static string[] gridStrings =
        [
            ".................................................................................",
            "123...476...123..88.....123.............4...........5.5...6............7...7.....",
            ".......1.1.....5....5..........5....3...........12..3....3............8....8.....",
            ".9.....8.5......96......4..6..34....9......2.2...6..17.1....8...6..17..97...95...",
            "...1............1...................1....................................1.......",
            "...81.....2........1.9..7...7..25.934.2............5...975.....563.....4......68.",
            "........5.2...9....9..2...373..481.....36....58....4...1...358...42.......978...2",
            "5...634.....7.....1...5.83.....18..7..69......43...9...............7..2.32.64.5..",
            "..3.8.5...4.....9.7..15.4..98...1..5..2.7983...5.6....1....6.5.8.....7..........4",
            "7.98.5...6..1..8.........9.8..4...1.....5..4.2....7..5..6...789.2.9..........3.6."
        ];

    static void Main()
    {
        Console.WriteLine("*** Brute force algorithm");
        MeasureAlgorithm(Solver.BruteForceAlgorithm, 100);

        Console.WriteLine("*** Bit algorithm");
        MeasureAlgorithm(Solver.BitAlgorithm, 100);

        Console.WriteLine("*** MVR algorithm");
        MeasureAlgorithm(Solver.MVRAlgorithm, 1000);
    }

    public static void MeasureAlgorithm(ISolvingAlgorithm algorithm, int repetitions)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        // Code to measure
        int solved = SolveNTimes(repetitions, algorithm);

        stopwatch.Stop();

        Console.WriteLine($"Time elapsed per repetition: {stopwatch.Elapsed.TotalMilliseconds / repetitions:F8} ms");
        Console.WriteLine($"Total Solved: {solved} / {gridStrings.Length}");
    }

    public static int SolveNTimes(int repetitions, ISolvingAlgorithm algorithm)
    {
        int solvedCount = 0;
        double[] gridClock = new double[gridStrings.Length];

        for (int grid = 0; grid < gridStrings.Length; grid++)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            bool solvedOnLast = false;

            for (int repetition = 0; repetition < repetitions; repetition++)
            {
                Grid currentGrid = Grid.CreateFromString(gridStrings[grid]);
                bool solved = Solver.SolveWith(currentGrid, algorithm);

                if (repetition == repetitions - 1 && solved)
                {
                    solvedOnLast = true;
                    solvedCount++;
                }
            }

            stopwatch.Stop();
            gridClock[grid] += stopwatch.Elapsed.TotalMilliseconds;

            if (solvedOnLast)
            {
                Console.WriteLine($"Solved puzzle {grid}, Average time: {gridClock[grid] / repetitions:F8} ms");
            }
        }

        return solvedCount;
    }

    static Grid[] CreateGrids()
    {
        Grid[] grids = new Grid[gridStrings.Length];
        for (int i = 0; i < gridStrings.Length; i++)
        {
            grids[i] = Grid.CreateFromString(gridStrings[i]);
        }

        return grids;
    }
}

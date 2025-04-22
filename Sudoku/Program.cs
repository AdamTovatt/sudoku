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
            "...1............1...................1....................................1......."
        ];

    static void Main()
    {
        Console.WriteLine("Brute force algorithm");
        MeasureAlgorithm(Solver.BruteForceAlgorithm);

        Console.WriteLine("Bit algorithm");
        MeasureAlgorithm(Solver.BitAlgorithm);

        Console.WriteLine("MVR algorithm");
        MeasureAlgorithm(Solver.MVRAlgorithm);
    }

    public static void MeasureAlgorithm(ISolvingAlgorithm algorithm)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        // Code to measure
        int solved = SolveNTimes(50, algorithm);

        stopwatch.Stop();

        Console.WriteLine($"Time elapsed: {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"Total Solved: {solved} / {5}");
    }

    public static int SolveNTimes(int repetitions, ISolvingAlgorithm algorithm)
    {
        int solvedCount = 0;
        for (int repetition = 0; repetition < repetitions; repetition++)
        {
            Grid[] grids = CreateGrids();
            for (int grid = 0; grid < grids.Length; grid++)
            {
                bool solved = Solver.SolveWith(grids[grid], algorithm);
                if (repetition == 0 && solved) solvedCount++;
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

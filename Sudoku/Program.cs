using System;
using System.Diagnostics;

using Sudoku;
using Sudoku.Solvers;

class Program
{
    static void Main()
    {
        Grid[] grids = CreateGrids();

        // Measure BruteForceAlgorithm
        Stopwatch stopwatch = Stopwatch.StartNew();

        // Code to measure
        SolveNTimes(100, new BruteForceAlgorithm(), grids);

        stopwatch.Stop();

        Console.WriteLine("Brute force algorithm");
        Console.WriteLine($"Time elapsed: {stopwatch.ElapsedMilliseconds} ms");

        // Measure BitAlgorithm
        stopwatch = Stopwatch.StartNew();

        // Code to measure
        SolveNTimes(100, new BitAlgorithm(), grids);

        stopwatch.Stop();

        Console.WriteLine("Bit algorithm");
        Console.WriteLine($"Time elapsed: {stopwatch.ElapsedMilliseconds} ms");
    }

    public static void SolveNTimes(int repetitions, ISolvingAlgorithm algorithm, Grid[] grids)
    {
        for (int repetition = 0; repetition < repetitions; repetition++)
        {
            for (int grid = 0; grid < 11; grid++)
            {
                Solver.SolveWith(grids[grid], algorithm);
            }
        }
    }

    static Grid[] CreateGrids()
    {
        string[] gridStrings = new string[]
        {
            "53..7....6..195....98....6.8...6...34..8..6..2...3...17.6....28....419..5....8..7",
            "6..874...9..5.3...13...6.9....6..4..17..9..62..4..1....3.6.4...85...9.2..1...728.",
            "..3.2.6..9..3.5..1..18.64..3.2.....1..7...6..2.....3.8..69.83..5..2.1.3..7..4.5..",
            ".2.6.8...58...97..9...6..4.4..8.3..5.7.....1.6..3.2..8.6..4...5..73...12...9.2.8.",
            ".6..7..28...195....8..6....4..8..3..7...4...1..5..2..9....7..3....6.843...12..9..",
            ".1.9..3..4..7.6.2..3...8.1..6...9...7..5...2..8...7...9..2.6...7..9.4.3..6..1..5.",
            "..5.9.1..3.8..2.5..6....3..9..7..6..2.4.....9.1..8..5..7..6....1..9.3..8.2..2.5..",
            "8..2..7..1..9.3..5..3...9..4..6...1..9...5...6..2...3..8..7...4..6..1.2..7..4..6.",
            ".3.8.5..1..9.2.6..8..6....3.7..4..9..5..1..2..9..6..1.4.2....8..7..3.4.1..5..7.6.",
            "..6.1..9.1..4..7..3.9....6..8..2..4..6.3.....1..7..5..2..8....3.4..9..6..5.2..3..",
            "4...1.96.9..5..26..3........2.3...8....3.6...7.8.1....5..1.9.2..4...8............"
        };

        Grid[] grids = new Grid[gridStrings.Length];
        for (int i = 0; i < gridStrings.Length; i++)
        {
            grids[i] = Grid.CreateFromString(gridStrings[i]);
        }

        return grids;
    }
}

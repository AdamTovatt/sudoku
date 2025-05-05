using System;
using System.Numerics;

namespace Sudoku.Solvers
{
    /// <summary>
    /// A backtracking Sudoku solver using the Minimum Remaining Value heuristic.
    /// Always picks the empty cell with the fewest possibilities first,
    /// and applies simple preprocessing to fill any forced cells.
    /// This is essentially an improved version of the MVR solver. It handles
    /// the logic of when the board is solved more efficiently.
    /// </summary>
    public class PreprocessAlgorithm : ISolvingAlgorithm
    {
        private Grid grid = null!;
        private const int BoardSidelength = 9;

        public PreprocessAlgorithm() { }
        public PreprocessAlgorithm(Grid grid)
        {
            this.grid = grid;
        }

        public bool SolveGrid(Grid grid)
        {
            this.grid = grid;
            Preprocess();
            return Solve();
        }

        /// <summary>
        /// Repeatedly fills any cell that has exactly one valid candidate.
        /// Stops when no more singletons can be found.
        /// </summary>
        private void Preprocess()
        {
            bool progress;
            do
            {
                progress = false;
                for (int y = 0; y < BoardSidelength; y++)
                {
                    for (int x = 0; x < BoardSidelength; x++)
                    {
                        if (!grid.IsCellEmpty(x, y)) continue;

                        int mask = ~(grid.columns[x] | grid.rows[y] | grid.squares[(x / 3) + y / 3 * 3]) & 0b111111111;
                        if (BitOperations.PopCount((uint)mask) == 1)
                        {
                            int digit = BitOperations.TrailingZeroCount(mask) + 1;
                            grid.SetCell(x, y, digit);
                            progress = true;
                        }
                    }
                }
            } while (progress);
        }

        /// <summary>
        /// Recursive solver: always picks the empty cell with the fewest options.
        /// </summary>
        private bool Solve()
        {
            var (bestX, bestY, bestMask) = FindMostConstrainedCell();

            // If no empty cell found, the puzzle is solved
            if (bestX == -1) return true;

            // Try each candidate in the best cell
            int bits = bestMask;
            while (bits != 0)
            {
                int pick = bits & -bits;
                int digit = BitOperations.TrailingZeroCount(pick) + 1;
                bits &= bits - 1;

                grid.SetCell(bestX, bestY, digit);
                if (Solve()) return true;
                grid.ClearCell(bestX, bestY);
            }

            return false;
        }

        /// <summary>
        /// Scans the board and returns the coordinates and mask of the empty cell
        /// with the fewest candidates. Returns (-1,-1,0) if no empties remain.
        /// </summary>
        private (int x, int y, int mask) FindMostConstrainedCell()
        {
            int bestX = -1, bestY = -1;
            int bestMask = 0;
            int minCount = BoardSidelength + 1;

            for (int y = 0; y < BoardSidelength; y++)
            {
                for (int x = 0; x < BoardSidelength; x++)
                {
                    if (!grid.IsCellEmpty(x, y)) continue;

                    int mask = ~(grid.columns[x] | grid.rows[y] | grid.squares[(x / 3) + y / 3 * 3]) & 0b111111111;
                    int count = BitOperations.PopCount((uint)mask);
                    if (count == 0) return (x, y, 0);
                    if (count == 1) return (x, y, mask);
                    if (count < minCount)
                    {
                        minCount = count;
                        bestMask = mask;
                        bestX = x;
                        bestY = y;
                    }
                }
            }

            return (bestX, bestY, bestMask);
        }
    }
}
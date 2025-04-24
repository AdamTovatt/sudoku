using System.Numerics;

namespace Sudoku.Solvers
{
    // The main differentiating factor of the MVR solver is that
    // within the recursive algorithm, it always chooses the square
    // with the least possibilities as the next square.
    public class MVRAlgorithm : ISolvingAlgorithm
    {
        private const int boardSideLength = 9;

        public MVRAlgorithm() { }

        public bool SolveGrid(Grid grid)
        {
            return Solve(grid, grid.DigitCount);
        }

        private (int x, int y, int mask)? FindCellWithFewestOptions(Grid grid)
        {
            int minimumOptions = boardSideLength + 1;
            (int x, int y, int mask) best = (-1, -1, 0);

            for (int x = 0; x < boardSideLength; x++)
            {
                for (int y = 0; y < boardSideLength; y++)
                {
                    if (grid.GetCell(x, y) != 0) continue;

                    int available = ~(grid.rows[y] | grid.columns[x] | grid.squares[(x / 3) + y / 3 * 3]) & 0b111111111;

                    int count = BitOperations.PopCount((uint)available);
                    if (count == 0) return null;
                    if (count == 1) return (x, y, available);

                    if (count < minimumOptions)
                    {
                        minimumOptions = count;
                        best = (x, y, available);
                    }
                }
            }

            return best.x == -1 ? null : best;
        }

        private bool Solve(Grid grid, int filled)
        {
            if (filled == 81)
                return true;

            (int x, int y, int mask)? next = FindCellWithFewestOptions(grid);
            if (next == null) return false;

            (int x, int y, int mask) = next.Value;

            for (int digit = 1; digit <= boardSideLength; digit++)
            {
                if ((mask & 1 << (digit - 1)) == 0) continue;

                grid.SetCell(x, y, digit);
                if (Solve(grid, filled + 1)) return true;
                grid.ClearCell(x, y); // Backtrack
            }

            return false;
        }
    }
}
using System.Numerics;

namespace Sudoku.Solvers
{
    /// <summary>
    /// The main differentiating factor of the MVR solver is that
    /// within the recursive algorithm, it always chooses the square
    /// with the least possibilities as the next square.
    /// </summary>
    public class MVRAlgorithm : ISolvingAlgorithm
    {
        private Grid grid = null!;
        private const int BoardSidelength = 9;

        public MVRAlgorithm() { }

        public MVRAlgorithm(Grid grid)
        {
            this.grid = grid;
        }

        public bool SolveGrid(Grid grid)
        {
            this.grid = grid;
            return Solve(grid.DigitCount);
        }

        public (int x, int y, int mask)? FindCellWithFewestOptions()
        {
            int minimumOptions = BoardSidelength + 1;
            (int x, int y, int mask) best = (-1, -1, 0);

            for (int x = 0; x < BoardSidelength; x++)
            {
                for (int y = 0; y < BoardSidelength; y++)
                {
                    if (grid.GetCell(x, y) != 0) continue;

                    int available = ~(grid.rows[y] | grid.columns[x] | grid.squares[(x / 3) + (y / 3) * 3]) & 0b111111111;

                    int count = BitOperations.PopCount((uint)available);
                    if (count == 0) return null; // early fail
                    if (count == 1) return (x, y, available); // early success

                    if (count < minimumOptions)
                    {
                        minimumOptions = count;
                        best = (x, y, available);
                    }
                }
            }

            return best.x == -1 ? null : best;
        }

        private bool Solve(int filled)
        {
            if (filled == 81)
                return true;

            var next = FindCellWithFewestOptions();
            if (next == null) return false;

            var (x, y, mask) = next.Value;

            for (int digit = 1; digit <= BoardSidelength; digit++)
            {
                if ((mask & 1 << (digit - 1)) == 0) continue;

                grid.SetCell(x, y, digit);
                if (Solve(filled + 1)) return true;
                grid.ClearCell(x, y); // Backtrack
            }

            return false;
        }
    }
}
using System.Numerics;

namespace Sudoku.Solvers
{
    /// <summary>
    /// Essentially a brute force solver, except that it doesn't use
    /// IsValid() for each possible digit, but instead gets a bitmask
    /// with all possible valid digits for a given position. This in
    /// itself does not make the recursion all that much faster,
    /// since the engine is still quite 'dumb'.
    /// The main differentiating factor that makes it the MVR solver 
    /// faster is that within the recursive algorithm, it always 
    /// chooses the square with the least possibilities (given a
    /// set of rules) as the next square to search.
    /// </summary>  // gets a bitmask with all valid digits.
    public class MVRAlgorithm : ISolvingAlgorithm
    {
        private const int boardSideLength = 9;

        public MVRAlgorithm() { }

        public bool SolveGrid(Grid grid)
        {
            return Solve(grid, grid.DigitCount);
        }

        /// <summary>
        /// Finds the cell in the grid that has the fewest possible valid
        /// digits that can be placed according to sudoku's rule zero.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
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

        /// <summary>
        /// A recursive method that goes over all empty squares in the
        /// sudoku board, trying digits until a solution is found.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="filled"></param>
        /// <returns></returns>
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
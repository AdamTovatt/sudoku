using System.Globalization;
using System.Numerics;

namespace Sudoku.Solvers
{
    public class BitAlgorithm : ISolvingAlgorithm
    {
        // Essentially a brute force solver, except that it doesn't
        // compute IsValid() for every digit but instead quickly
        // gets a bitmask with all valid digits. All code here is
        // very efficient, but the Solver is not much smarter than
        // BruteForce so it doesn't help much.
        private Grid grid = null!;
        private const int BoardSidelength = 9;

        public bool SolveGrid(Grid grid)
        {
            this.grid = grid;
            return BruteForceSolveGrid();
        }

        public bool BruteForceSolveGrid()
        {
            return Solve(0, 0);
        }

        private bool Solve(int x, int y)
        {
            if (x == 9) { x = 0; y++; }
            if (y == 9) return true;

            // If square is already filled
            if (grid.GetCell(x, y) != 0) return Solve(x + 1, y);

            int availableDigits = ~(grid.rows[y] | grid.columns[x] | grid.squares[(x / 3) + y / 3 * 3]) & 0x1FF;

            for (int digit = 1; digit <= 9; digit++)
            {
                if ((availableDigits & 1 << (digit - 1)) == 0) continue;

                grid.SetCell(x, y, digit);
                if (Solve(x + 1, y)) return true;
                grid.ClearCell(x, y); //  Backtrack
            }

            return false;
        }
    }
}
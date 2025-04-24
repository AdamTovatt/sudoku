using System.Globalization;
using System.Numerics;

namespace Sudoku.Solvers
{
    // This implementation of the MVR Solver does extra checks for
    // hidden singles. Might be faster than the basic MVR Algorithm
    // for more advanced sudoku's, but I guess we'll see.
    public class LoadAssociatedAlgorithm : ISolvingAlgorithm
    {
        private Grid grid = null!;
        private const int BoardSidelength = 9;
        private static readonly int[,,] AssociatedStructures = Int3DArrayLoader.LoadArray("associated.bin");

        public LoadAssociatedAlgorithm() { }

        public LoadAssociatedAlgorithm(Grid grid)
        {
            this.grid = grid;
        }

        public bool SolveGrid(Grid grid)
        {
            this.grid = grid;
            while (PlaceAllGuaranteed()) ;
            return Solve(grid.DigitCount);
        }

        private bool PlaceAllGuaranteed()
        {
            bool placed = false;

            for (int x = 0; x < BoardSidelength; x++)
            {
                for (int y = 0; y < BoardSidelength; y++)
                {
                    if (grid.GetCell(x, y) != 0) continue;

                    int available = ~(grid.rows[y] | grid.columns[x] | grid.squares[(x / 3) + (y / 3) * 3]) & 0b111111111;

                    if (BitOperations.PopCount((uint)available) == 1)
                    {
                        placed = true;
                        grid.SetCell(x, y, BitOperations.TrailingZeroCount((uint)available) + 1);
                        continue;
                    }

                    // One single option, or none
                    int possibleMask = available
                        & grid.columns[AssociatedStructures[x, y, 0]]
                        & grid.columns[AssociatedStructures[x, y, 1]]
                        & grid.rows[AssociatedStructures[x, y, 2]]
                        & grid.rows[AssociatedStructures[x, y, 3]];

                    if (possibleMask != 0)
                    {
                        placed = true;
                        grid.SetCell(x, y, BitOperations.TrailingZeroCount((uint)possibleMask) + 1);
                    }
                }
            }

            return placed;
        }

        private (int x, int y, int mask)? FindCellWithFewestOptions()
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

                    // One single option, or none
                    int possibleMask = available
                        & grid.columns[AssociatedStructures[x, y, 0]]
                        & grid.columns[AssociatedStructures[x, y, 1]]
                        & grid.rows[AssociatedStructures[x, y, 2]]
                        & grid.rows[AssociatedStructures[x, y, 3]];

                    if (possibleMask != 0) return (x, y, possibleMask);

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
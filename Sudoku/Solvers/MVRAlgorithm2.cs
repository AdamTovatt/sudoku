using System.Numerics;

namespace Sudoku.Solvers
{
    // The main differentiating factor of the MVR solver is that
    // within the recursive algorithm, it always chooses the square
    // with the least possibilities as the next square.
    public class MVRAlgorithm2 : ISolvingAlgorithm
    {
        private const int boardSideLength = 9;

        public MVRAlgorithm2() { }

        public bool SolveGrid(Grid grid)
        {
            return Solve(grid, grid.DigitCount);
        }

        private (int x, int y, int mask)? FindCellWithFewestOptions(Grid grid)
        {
            int minimumOptions = boardSideLength + 1;
            (int x, int y, int mask) best = (-1, -1, 0);

            for (int boxY = 0; boxY < 3; boxY++)
            {
                for (int boxX = 0; boxX < 3; boxX++)
                {
                    int columnBoxStart = boxX * 3;
                    int rowBoxStart = boxY * 3;

                    for (int dy = 0; dy < 3; dy++)
                    {
                        for (int dx = 0; dx < 3; dx++)
                        {
                            int x = columnBoxStart + dx;
                            int y = rowBoxStart + dy;

                            if (grid.GetCell(x, y) != 0) continue;

                            int available = ~(grid.rows[y] | grid.columns[x] | grid.squares[boxX + boxY * 3]) & 0b111111111;

                            int count = BitOperations.PopCount((uint)available);
                            if (count == 0) return null;
                            if (count == 1) return (x, y, available);

                            // Hidden single approximation (as before, could still be improved)
                            int possibleMask = available
                                & grid.columns[columnBoxStart + (dx + 1) % 3]
                                & grid.columns[columnBoxStart + (dx + 2) % 3]
                                & grid.rows[rowBoxStart + (dy + 1) % 3]
                                & grid.rows[rowBoxStart + (dy + 2) % 3];

                            if (BitOperations.PopCount((uint)possibleMask) == 1)
                                return (x, y, possibleMask);

                            if (count < minimumOptions)
                            {
                                minimumOptions = count;
                                best = (x, y, available);
                            }
                        }
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
using System.Numerics;
using System.Security.Cryptography.X509Certificates;

namespace Sudoku.Solvers
{
    // The main differentiating factor of the MVR solver is that
    // within the recursive algorithm, it always chooses the square
    // with the least possibilities as the next square.
    public class PreprocessAlgorithm : ISolvingAlgorithm
    {
        private Grid grid = null!;
        private const int BoardSidelength = 9;
        private int[,] ValidCandidates = new int[9, 9];

        public PreprocessAlgorithm() { }

        public PreprocessAlgorithm(Grid grid)
        {
            this.grid = grid;
        }

        public bool SolveGrid(Grid grid)
        {
            this.grid = grid;
            for (int x = 0; x < BoardSidelength; x++)
            {
                for (int y = 0; y < BoardSidelength; y++)
                {
                    ValidCandidates[x, y] = 0b111111111;
                }
            }
            Preprocess(grid);
            return Solve(0, 0);
        }

        /// <summary>
        /// Updates the ValidCandidates for each cell.
        /// Fills all squares with only one alternative directly.
        /// </summary>
        /// <param name="grid"></param>
        public void Preprocess(Grid grid)
        {
            bool progressMade;

            do
            {
                progressMade = false;

                for (int x = 0; x < BoardSidelength; x++)
                {
                    for (int y = 0; y < BoardSidelength; y++)
                    {
                        int available = ~(grid.rows[y] | grid.columns[x] | grid.squares[(x / 3) + y / 3 * 3]) & 0b111111111;

                        if (ValidCandidates[x, y] != available)
                        {
                            if (BitOperations.PopCount((uint)available) + 1 == 1)
                            {
                                grid.SetCell(x, y, BitOperations.TrailingZeroCount(available) + 1);
                                progressMade = true;
                                ValidCandidates[x, y] = 0;
                                continue;
                            }

                            ValidCandidates[x, y] = available;
                        }
                    }
                }

            } while (progressMade);
        }

        private bool Solve(int x, int y)
        {
            if (x == 9) { x = 0; y++; }
            if (y == 9) return true;

            // If square is already filled
            if (grid.IsCellEmpty(x, y)) return Solve(x + 1, y);

            for (int digit = 1; digit <= 9; digit++)
            {
                if ((ValidCandidates[x, y] & 1 << (digit - 1)) == 0) continue;

                grid.SetCell(x, y, digit);
                if (Solve(x + 1, y)) return true;
                grid.ClearCell(x, y); //  Backtrack
            }

            return false;
        }
    }
}
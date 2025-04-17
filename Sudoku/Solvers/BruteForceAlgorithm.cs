namespace Sudoku.Solvers
{
    public class BruteForceAlgorithm : ISolvingAlgorithm
    {
        public bool SolveGrid(Grid grid)
        {
            return Solve(grid, 0, 0);
        }

        private bool Solve(Grid grid, int row, int column)
        {
            if (column == grid.GetSideLength()) { column = 0; row++; }
            if (row == grid.GetSideLength()) return true;

            // If the digit is already filled in
            if (grid.GetCell(column, row) != 0) return Solve(grid, row, column + 1);

            // Tries new numbers if square is empty
            else
            {
                for (int digit = 1; digit <= 9; digit++)
                {
                    if (grid.IsValid(column, row, digit))
                    {
                        grid.SetCell(column, row, digit);
                        if (Solve(grid, row, column + 1)) return true;
                        grid.ClearCell(column, row); // Backtrack
                    }
                }
            }

            return false;
        }
    }
}
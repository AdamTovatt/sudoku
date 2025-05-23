namespace Sudoku.Solvers
{
    /// <summary>
    /// A standard brute force solver for sudoku that uses the
    /// Grid class to represent the sudoku board.
    /// </summary>
    public class BruteForceAlgorithm : ISolvingAlgorithm
    {
        public bool SolveGrid(Grid grid)
        {
            return Solve(grid, 0, 0);
        }

        /// <summary>
        /// A recursive algorithm that goes over all squares in the sudoku
        /// one by one, testing digit after digit in each square until
        /// it stumbles upon a solution that works.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private bool Solve(Grid grid, int row, int column)
        {
            if (column == grid.SideLength) { column = 0; row++; }
            if (row == grid.SideLength) return true;

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
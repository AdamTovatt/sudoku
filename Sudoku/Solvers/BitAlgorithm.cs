namespace Sudoku.Solvers
{
    public class BitAlgorithm : ISolvingAlgorithm
    {
        private BruteForceAlgorithm backupSolver = new BruteForceAlgorithm();

        private int boardSidelength = 9;

        public bool SolveGrid(Grid grid)
        {
            while (PlaceAllGuaranteed(grid)) ;
            return backupSolver.SolveGrid(grid);
        }

        private bool PlaceGuaranteed(Grid grid, int x, int y, int digit)
        {
            int rowBoxStart = x / 3 * 3;
            int colBoxStart = y / 3 * 3;
            int row1 = rowBoxStart + ((x + 1) % 3);
            int row2 = rowBoxStart + ((x + 2) % 3);
            int column1 = colBoxStart + ((y + 1) % 3);
            int column2 = colBoxStart + ((y + 2) % 3);

            if (grid.IsDigitInRow(row1, digit) &&
                grid.IsDigitInRow(row2, digit) &&
                grid.IsDigitInColumn(column1, digit) &&
                grid.IsDigitInColumn(column2, digit))
            {
                grid.SetCell(x, y, digit);
                return true;
            }

            return false;
        }

        private bool PlaceAllGuaranteed(Grid grid)
        {
            bool placed = false;
            for (int x = 0; x < boardSidelength; x++)
            {
                for (int y = 0; y < boardSidelength; y++)
                {

                    if (grid.GetCell(x, y) != 0) continue;

                    for (int digit = 1; digit <= boardSidelength; digit++)
                    {
                        if (!grid.IsValid(x, y, digit)) continue;

                        if (PlaceGuaranteed(grid, x, y, digit))
                        {
                            placed = true;
                            break;
                        }
                    }

                }
            }
            return placed;
        }
    }
}
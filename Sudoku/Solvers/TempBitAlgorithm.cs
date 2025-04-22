using System.Numerics;

namespace Sudoku.Solvers
{
    public class TempBitAlgorithm : ISolvingAlgorithm
    {
        private Grid grid = null!;
        private const int BoardSidelength = 9;
        private Cell[,] cells = InitCells();
        private static Cell[,] InitCells()
        {
            var grid = new Cell[BoardSidelength, BoardSidelength];
            for (int x = 0; x < BoardSidelength; x++)
                for (int y = 0; y < BoardSidelength; y++)
                    grid[x, y] = new Cell();
            return grid;
        }

        public bool SolveGrid(Grid grid)
        {
            this.grid = grid;
            while (PlaceAllGuaranteed()) ;
            return BruteForceSolveGrid();
        }

        private bool PlaceAllGuaranteed()
        {
            bool placed = false;

            while (FillNearlyCompleteStructures())
            {
                placed = true;
            }

            for (int x = 0; x < BoardSidelength; x++)
            {
                for (int y = 0; y < BoardSidelength; y++)
                {
                    if (grid.GetCell(x, y) != 0) continue;

                    for (int digit = 1; digit <= BoardSidelength; digit++)
                    {
                        if (!grid.IsValid(x, y, digit))
                        {
                            cells[x, y].RemoveDigit(digit);
                            continue;
                        }

                        if (PlaceGuaranteed(x, y, digit))
                        {
                            placed = true;
                            break;
                        }
                    }
                }
            }
            return placed;
        }

        private bool PlaceGuaranteed(int x, int y, int digit)
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

        private bool FillNearlyCompleteStructures()
        {
            bool placed = false;
            int fullMask = 0b111111111;

            for (int index = 0; index < BoardSidelength; index++)
            {

                // Complete row if row is nearly full
                if (BitOperations.PopCount((uint)grid.rows[index]) == BoardSidelength - 1)
                {
                    int usedMask = grid.rows[index];
                    int missingBit = fullMask ^ usedMask;
                    int missingDigit = BitOperations.TrailingZeroCount((uint)missingBit) + 1;

                    for (int column = 0; column < 9; column++)
                    {
                        if (grid.GetCell(column, index) == 0)
                        {
                            grid.SetCell(column, index, missingDigit);
                            placed = true;
                            break;
                        }
                    }
                }

                // Complete column if column is nearly full
                if (BitOperations.PopCount((uint)grid.columns[index]) == BoardSidelength - 1)
                {
                    int usedMask = grid.columns[index];
                    int missingBit = fullMask ^ usedMask;
                    int missingDigit = BitOperations.TrailingZeroCount((uint)missingBit) + 1;

                    for (int row = 0; row < 9; row++)
                    {
                        if (grid.GetCell(index, row) == 0)
                        {
                            grid.SetCell(index, row, missingDigit);
                            placed = true;
                            break;
                        }
                    }
                }

                // Complete square if square is nearly full
                if (BitOperations.PopCount((uint)grid.squares[index]) == 8)
                {
                    int usedMask = grid.squares[index];
                    int missingBit = fullMask ^ usedMask;
                    int missingDigit = BitOperations.TrailingZeroCount((uint)missingBit) + 1;

                    int squareRowStart = index / 3 * 3;
                    int squareColStart = index % 3 * 3;

                    bool found = false;

                    for (int x = squareColStart; x < squareColStart + 3 && !found; x++)
                    {
                        for (int y = squareRowStart; y < squareRowStart + 3; y++)
                        {
                            if (grid.GetCell(x, y) == 0)
                            {
                                grid.SetCell(x, y, missingDigit);
                                placed = true;
                                found = true;
                                break;
                            }
                        }
                    }
                }
            }

            return placed;
        }

        public bool BruteForceSolveGrid()
        {
            return Solve(0, 0);
        }

        private bool Solve(int column, int row)
        {
            if (column == grid.SideLength) { column = 0; row++; }
            if (row == grid.SideLength) return true;

            // If the digit is already filled in
            if (grid.GetCell(column, row) != 0) return Solve(column + 1, row);

            // Tries new numbers if square is empty
            else
            {
                foreach (int digit in cells[column, row].PossibleDigits)
                {
                    if (grid.IsValid(column, row, digit))
                    {
                        grid.SetCell(column, row, digit);
                        if (Solve(column + 1, row)) return true;
                        grid.ClearCell(column, row); // Backtrack
                    }
                }
            }

            return false;
        }

        private class Cell
        {
            public HashSet<int> PossibleDigits { get; private set; }

            public Cell()
            {
                // Initialize with digits 1 through 9
                PossibleDigits = Enumerable.Range(1, 9).ToHashSet();
            }

            public void RemoveDigit(int digit)
            {
                PossibleDigits.Remove(digit);
            }

            public void Reset()
            {
                PossibleDigits = Enumerable.Range(1, 9).ToHashSet();
            }

            public bool HasOnlyOnePossibility()
            {
                return PossibleDigits.Count == 1;
            }

            public int GetOnlyPossibility()
            {
                return PossibleDigits.First(); // Only call if HasOnlyOnePossibility() is true
            }
        }
    }
}
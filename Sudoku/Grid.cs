using System.Text;

namespace Sudoku
{
    public class Grid
    {
        private int[,] grid;
        private int[] rows;
        private int[] columns;
        private int[] squares;

        public int SideLength;

        public Grid(int sideLength)
        {
            SideLength = sideLength;
            grid = new int[sideLength, sideLength];
            rows = new int[sideLength];
            columns = new int[sideLength];
            squares = new int[sideLength];
        }

        public static Grid CreateFromString(string gridString, int sideLength = 9)
        {
            if (gridString.Length != sideLength * sideLength) // Ensure length of the gridString is valid
                throw new ArgumentException($"Grid string length does not match the expected grid size ({sideLength * sideLength}).");

            Grid grid = new Grid(sideLength);
            int index = 0;

            for (int y = 0; y < sideLength; y++)
            {
                for (int x = 0; x < sideLength; x++)
                {
                    char character = gridString[index];
                    index++;

                    if (character == '0' || character == '.') continue; // Skip empty cells

                    if (character >= '1' && character <= '9')
                    {
                        grid.SetCell(x, y, character - '0'); // Convert char digit to int
                    }
                    else
                    {
                        throw new ArgumentException($"Invalid character '{character}' at position {index - 1}.");
                    }
                }
            }

            return grid;
        }

        public bool HasSameCellValuesAs(Grid otherGrid)
        {
            // Check that they are the same size
            if (SideLength != otherGrid.GetSideLength()) return false;

            // Match each element
            for (int y = 0; y < SideLength; y++)
            {
                for (int x = 0; x < SideLength; x++)
                {
                    if (GetCell(x, y) != otherGrid.GetCell(x, y)) return false;
                }
            }

            return true;
        }

        public void SetCell(int x, int y, int value)
        {
            if (value < 0 || value > SideLength)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"Value must be between 0 and 9 (0 means empty).");
            }

            if (x < 0 || x >= SideLength)
                throw new ArgumentOutOfRangeException(nameof(x), $"The index \"{x}\" is not valid. Must be between 0 and {SideLength}.");

            if (y < 0 || y >= SideLength)
                throw new ArgumentOutOfRangeException(nameof(y), $"The index \"{y}\" is not valid. Must be between 0 and {SideLength}.");

            // Update the cell value
            grid[x, y] = value;

            int mask = 1 << (value - 1);
            int square = y / 3 * 3 + (x / 3);

            // Set the bit in the corresponding row, column, and block
            // This is the only extra thing that has to be done from 'OriginalGrid'
            rows[y] |= mask;
            columns[x] |= mask;
            squares[square] |= mask;
        }

        public int GetCell(int x, int y)
        {
            return grid[x, y];
        }

        public bool IsCellEmpty(int x, int y)
        {
            return grid[x, y] == 0;
        }

        public int GetSideLength()
        {
            return SideLength;
        }

        public bool IsValid(int x, int y, int digit)
        {
            int squareIndex = (y / 3) * 3 + (x / 3);
            int mask = 1 << (digit - 1);
            return (rows[y] & mask) == 0 &&
                  (columns[x] & mask) == 0 &&
                  (squares[squareIndex] & mask) == 0;
        }

        public void ClearCell(int x, int y)
        {
            int value = grid[x, y];

            // Clear the cell
            grid[x, y] = 0;

            int mask = ~(1 << (value - 1));

            // Clear the bit in the bitmasks
            rows[y] &= mask;
            columns[x] &= mask;
            squares[y / 3 * 3 + (x / 3)] &= mask;
        }

        public bool IsDigitInRow(int row, int digit)
        {
            return (rows[row] & (1 << digit - 1)) != 0;
        }

        public bool IsDigitInColumn(int column, int digit)
        {
            return (columns[column] & (1 << digit - 1)) != 0;
        }

        public bool IsDigitInSquare(int x, int y, int digit)
        {
            int squareIndex = y / 3 * 3 + (x / 3);
            return (squares[squareIndex] & (1 << (digit - 1))) != 0;
        }

        public bool IsSolved(out InvalidCellInformation? invalidCellInformation)
        {
            for (int x = 0; x < SideLength; x++)
            {
                for (int y = 0; y < SideLength; y++)
                {
                    int cellValue = GetCell(x, y);

                    // TODO: Optimize this so it doesn't have to ClearCell and then SetCell
                    // each time. Don't change IsValid, since IsValid is more important that
                    // it's fast.
                    ClearCell(x, y);

                    if (cellValue == 0 || !IsValid(x, y, cellValue))
                    {
                        SetCell(x, y, cellValue);
                        invalidCellInformation = new InvalidCellInformation(x, y, cellValue);
                        return false;
                    }

                    SetCell(x, y, cellValue);
                }
            }

            invalidCellInformation = null;
            return true;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder(SideLength * SideLength * 2); // * 2 since we add spaces

            for (int y = 0; y < SideLength; y++)
            {
                for (int x = 0; x < SideLength; x++)
                {
                    stringBuilder.Append((char)('0' + GetCell(x, y)));
                    stringBuilder.Append(' ');
                }
                stringBuilder.Append('\n');
            }

            return stringBuilder.ToString();
        }
    }
}
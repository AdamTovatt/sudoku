using System.Text;

namespace Sudoku
{
    /// <summary>
    /// Represents a Sudoku grid. Like the "board" of a Sudoku.
    /// </summary>
    public class BitBoardGrid
    {
        // Represents the length of the grid's side
        public int SideLength { get; }
        public int TotalCells { get; }

        private readonly ulong[,] digitBitboards;

        public BitBoardGrid(int sideLength = 9) // let's default the side length to 9 since we only care about that now anyway
        {
            SideLength = sideLength;
            TotalCells = sideLength * sideLength;
            digitBitboards = new ulong[sideLength + 1, (sideLength * sideLength + 63) / 64];
        }

        public static BitBoardGrid CreateFromString(string gridString, int SideLength = 9)
        {
            // Check if the length of the gridString is valid
            if (gridString.Length != SideLength * SideLength)
                throw new ArgumentException("Grid string length does not match the expected grid size.");

            BitBoardGrid grid = new BitBoardGrid(SideLength);

            for (int i = 0; i < gridString.Length; i++)
            {
                char c = gridString[i];
                if (c == '0' | c == '.') continue; // Skip empty cells

                if (c >= '1' && c <= '0' + SideLength)
                {
                    int digit = c - '0';
                    if (digit >= 1 && digit <= SideLength)
                    {
                        grid.SetCell(i, digit); // Set the digit if it's within the allowed range
                    }
                }
                else
                    throw new ArgumentException($"Invalid character '{c}' in grid string.");
            }

            return grid;
        }

        public bool HasSameCellValuesAs(BitBoardGrid otherGrid)
        {
            // Check that they are the same size
            if (SideLength != otherGrid.SideLength) return false;

            // Match each element
            for (int i = 0; i < TotalCells; i++)
            {
                if (GetCell(i) != otherGrid.GetCell(i)) return false;
            }

            return true;
        }



        public void SetCell(int column, int row, int digit)
        {
            SetCell(row * SideLength + column, digit);
        }

        private void SetCell(int position, int digit)
        {
            if (digit < 1 || digit > SideLength)
                throw new ArgumentOutOfRangeException(nameof(digit), $"Digit must be between 1 and {SideLength}.");

            if (position < 0 || position >= TotalCells)
                throw new ArgumentOutOfRangeException(nameof(position), "Position must be within the valid range.");

            int bitboardNumber = position / 64;
            int newPosition = position - bitboardNumber * 64;

            // Set the corresponding bit in the digit's bitboard
            SetBit(ref digitBitboards[digit, bitboardNumber], newPosition);

            // Clear the corresponding bit in the emptyCellsBitboard (digitBitboards[0])
            SetBit(ref digitBitboards[0, bitboardNumber], newPosition);
        }

        public int GetCell(int column, int row)
        {
            return GetCell(row * SideLength + column);
        }

        private int GetCell(int position)
        {
            int bitboardNumber = position / 64;
            int newPosition = position - bitboardNumber * 64;

            // Check if the cell is empty (bit in digitBitboards[0] is set)
            if (IsCellEmpty(position)) return 0;

            // Check each digit bitboard to find the digit at the given position
            for (int digit = 1; digit <= SideLength; digit++)
            {
                if (GetBit(digitBitboards[digit, bitboardNumber], position) != 0)
                    return digit;
            }
            return 0; // Should never happen
        }

        public bool IsCellEmpty(int column, int row)
        {
            return IsCellEmpty(row * SideLength + column);
        }

        private bool IsCellEmpty(int position)
        {
            int bitboardNumber = position / 64;
            int newPosition = position - bitboardNumber * 64;

            return GetBit(digitBitboards[0, bitboardNumber], newPosition) == 0;
        }

        public ulong GetRow(int row, int digit)
        {
            ulong rowBitboard = 0;

            // Iterate through the columns
            for (int column = 0; column < SideLength; column++)
            {
                int position = row * SideLength + column;

                int bitboardNumber = position / 64;
                int newPosition = position - bitboardNumber * 64;

                if (GetBit(digitBitboards[digit, bitboardNumber], newPosition) != 0)
                {
                    rowBitboard |= (1UL << column);
                }
            }

            return rowBitboard;
        }

        public ulong GetColumn(int column, int digit)
        {
            ulong colBitboard = 0;

            // Iterate through the rows (0 to SideLength-1)
            for (int row = 0; row < SideLength; row++)
            {
                int position = row * SideLength + column;

                int bitboardNumber = position / 64;
                int newPosition = position - bitboardNumber * 64;

                if (GetBit(digitBitboards[digit, bitboardNumber], newPosition) != 0)
                {
                    colBitboard |= (1UL << row);
                }
            }

            return colBitboard;
        }

        public ulong GetSquare(int squareIndex, int digit)
        {
            ulong squareBitboard = 0;

            // Determine the top-left corner of the square
            int startRow = (squareIndex / 3) * 3;
            int startColumn = (squareIndex % 3) * 3;

            // Iterate over all 9 cells in the 3x3 square
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    int row = startRow + i;
                    int column = startColumn + j;
                    int position = row * SideLength + column;

                    int bitboardNumber = position / 64;
                    int newPosition = position - bitboardNumber * 64;

                    if (GetBit(digitBitboards[digit, bitboardNumber], newPosition) != 0)
                    {
                        squareBitboard |= (1UL << (i * 3 + j));
                    }
                }
            }

            return squareBitboard;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int column = 0; column < SideLength; column++)
            {
                for (int row = 0; row < SideLength; row++)
                {
                    stringBuilder.Append(GetCell(row, column).ToString());
                    stringBuilder.Append(" ");
                }
                stringBuilder.Append("\n");
            }
            return stringBuilder.ToString();
        }

        // Method to set the bit at a specific position to 1
        private void SetBit(ref ulong x, int position)
        {
            x |= (1UL << position);  // Set the bit at position to 1
        }

        // Method to clear the bit at a specific position (set it to 0)
        private void ClearBit(ref ulong x, int position)
        {
            x &= ~(1UL << position);  // Clear the bit at position (set it to 0)
        }

        // Method to get the value of the bit at a specific position (1 or 0)
        public int GetBit(ulong x, int position)
        {
            return ((x >> position) & 1) == 1 ? 1 : 0;  // Return 1 if bit is set, else 0
        }

    }
}
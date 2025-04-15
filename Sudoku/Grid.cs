using System.Text;

namespace Sudoku
{
    /// <summary>
    /// Represents a Sudoku grid. Like the "board" of a Sudoku.
    /// </summary>
    public class Grid
    {
        // Represents the length of the grid's side
        public int SideLength { get; }

        private readonly ulong[,] digitBitboards;

        public Grid(int sideLength = 9) // let's default the side length to 9 since we only care about that now anyway
        {
            SideLength = sideLength;
            digitBitboards = new ulong[sideLength + 1, (sideLength * sideLength + 63) / 64];
        }

        public static Grid CreateFromString(string gridString, int SideLength = 9)
        {
            // Check if the length of the gridString is valid
            if (gridString.Length != SideLength * SideLength)
                throw new ArgumentException("Grid string length does not match the expected grid size.");

            Grid grid = new Grid(SideLength);

            for (int i = 0; i < gridString.Length; i++)
            {
                char c = gridString[i];
                if (c == '0' | c == '.') continue; // Skip empty cells

                if (c >= '1' && c <= '0' + SideLength)
                {
                    int digit = c - '0';
                    if (digit >= 1 && digit <= SideLength)
                    {
                        grid.SetCell(i % SideLength, i / SideLength, digit); // Set the digit if it's within the allowed range
                    }
                }
                else
                    throw new ArgumentException($"Invalid character '{c}' in grid string.");
            }

            return grid;
        }

        public bool HasSameCellValuesAs(Grid otherGrid)
        {
            // Check that they are the same size
            if (SideLength != otherGrid.SideLength) return false;

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

        // Return true if the grid contains digit at positions given by the bitmask.
        // For example, if we want to check for the number 4 along an entire row,
        // we use a bitmask that symbolises the row and pass it in here.
        public bool ContainsDigit(int digit, ulong[] bitmask)
        {
            return !((digitBitboards[digit, 0] & bitmask[0]) == 0 & (digitBitboards[digit, 1] & bitmask[1]) == 0);
        }

        public void SetCell(int column, int row, int digit)
        {
            if (digit < 1 || digit > SideLength)
                throw new ArgumentOutOfRangeException($"Digit must be between 1 and {SideLength}.");

            int position = row * SideLength + column;

            if (position < 0 || position >= SideLength * SideLength)
                throw new ArgumentOutOfRangeException("Position must be within the valid range.");

            int bitboardNumber = position / 64;
            int newPosition = position % 64;

            // Set the corresponding bit in the digit's bitboard
            // The OR operator sets it to 1 even if it already was
            digitBitboards[digit, bitboardNumber] |= (1UL << newPosition);

            // Clear the corresponding bit in the emptyCellsBitboard (digitBitboards[0])
            digitBitboards[0, bitboardNumber] |= (1UL << newPosition);
        }

        public int GetCell(int column, int row)
        {
            int position = row * SideLength + column;
            int bitboardNumber = position / 64;
            int newPosition = position % 64;

            if (IsCellEmpty(column, row)) return 0;

            // Sadly not very efficient since we don't know what digit we're looking for,
            // and there is not way to just get it like if we used an array.
            ulong bitmask = 1UL << newPosition;
            for (int digit = 1; digit <= SideLength; digit++)
            {
                // This is essentially saying that if the current bitboard has a value at
                // the correct position, then the matching digit will be returned.
                if ((digitBitboards[digit, bitboardNumber] & bitmask) != 0)
                    return digit;
            }

            return 0; // Shouldn't happen
        }

        public bool IsCellEmpty(int column, int row)
        {
            int position = row * SideLength + column;

            // Check if the 'all digits bitboard' is filled in at the position
            return (digitBitboards[0, position / 64] & (1UL << position % 64)) == 0;
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
    }
}
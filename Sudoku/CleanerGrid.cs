using System.Data;
using System.Text;

namespace Sudoku
{
   public class CleanerGrid : IGrid
   {
      private int[,] grid;
      private int[] rows;
      private int[] columns;
      private int[] squares;

      public int SideLength;

      public CleanerGrid(int sideLength)
      {
         SideLength = sideLength;
         grid = new int[sideLength, sideLength];
         rows = new int[sideLength];
         columns = new int[sideLength];
         squares = new int[sideLength];
      }

      public static IGrid CreateFromString(string gridString, int sideLength = 9)
      {
         // Check if the length of the gridString is valid
         if (gridString.Length != sideLength * sideLength)
            throw new ArgumentException("Grid string length does not match the expected grid size (81).");

         CleanerGrid grid = new CleanerGrid(sideLength);
         int index = 0;

         for (int y = 0; y < sideLength; y++)
         {
            for (int x = 0; x < sideLength; x++)
            {
               char c = gridString[index];
               index++;

               if (c == '0' || c == '.') continue; // Skip empty cells

               if (c >= '1' && c <= '0' + sideLength)
               {
                  grid.SetCell(x, y, c - '0'); // Convert char digit to int
               }
               else
               {
                  throw new ArgumentException($"Invalid character '{c}' at position {index - 1}.");
               }
            }
         }

         return grid;
      }

      public bool HasSameCellValuesAs(IGrid otherGrid)
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

      private bool IsValid(int x, int y, int digit)
      {
         int squareIndex = (y / 3) * 3 + (x / 3);
         int mask = 1 << digit;
         return (rows[y] & mask) == 0 &&
               (columns[x] & mask) == 0 &&
               (squares[squareIndex] & mask) == 0;
      }

      public bool IsDigitInRow(int row, int digit)
      {
         return (rows[row] & (1 << digit)) != 0;
      }

      public bool IsDigitInCol(int column, int digit)
      {
         return (columns[column] & (1 << digit)) != 0;
      }

      public bool IsDigitInSquare(int x, int y, int digit)
      {
         int squareIndex = y / 3 * 3 + (x / 3);
         return (squares[squareIndex] & (1 << (digit - 1))) != 0;
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
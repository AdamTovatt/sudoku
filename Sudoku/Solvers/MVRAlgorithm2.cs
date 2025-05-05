using System.Numerics;
using System.IO;

namespace Sudoku.Solvers
{
   /// <summary>
   /// The main differentiating factor of the MVR solver is that
   /// within the recursive algorithm, it always chooses the square
   /// with the least possibilities as the next square.
   /// </summary>
   public class MVRAlgorithm2 : ISolvingAlgorithm
   {
      private Grid grid = null!;
      private const int BoardSidelength = 9;

      public MVRAlgorithm2() { }

      public MVRAlgorithm2(Grid grid)
      {
         this.grid = grid;
      }

      public bool SolveGrid(Grid grid)
      {
         this.grid = grid;
         return Solve(grid.DigitCount);
      }

      private (int x, int y, int mask)? FindCellWithFewestOptions()
      {
         int minimumOptions = BoardSidelength + 1;
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

                     int available = ~(grid.columns[x] | grid.rows[y] | grid.squares[boxX + boxY * 3]) & 0b111111111;

                     int count = BitOperations.PopCount((uint)available);
                     if (count == 0) return null;
                     if (count == 1) return (x, y, available);

                     // Hidden single approximation (as before, could still be improved)
                     int possibleMask = available
                         & grid.columns[columnBoxStart + (dx + 1) % 3]
                         & grid.columns[columnBoxStart + (dx + 2) % 3]
                         & grid.rows[rowBoxStart + (dy + 1) % 3]
                         & grid.rows[rowBoxStart + (dy + 2) % 3];

                     if (BitOperations.PopCount((uint)possibleMask) != 0)
                     {
                        File.AppendAllText("improveMVR2.csv", $"{BitOperations.TrailingZeroCount(possibleMask) + 1},{Convert.ToString(available, 2).PadLeft(0, '0')},{count},{minimumOptions}{Environment.NewLine}");
                        return (x, y, possibleMask);
                     }

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

      private bool Solve(int filled)
      {
         if (filled == 81)
            return true;

         var next = FindCellWithFewestOptions();
         if (next == null) return false;

         var (x, y, mask) = next.Value;

         int bits = mask;
         while (bits != 0)
         {
            int pick = bits & -bits;
            int digit = BitOperations.TrailingZeroCount(pick) + 1;
            bits &= bits - 1;

            grid.SetCell(x, y, digit);
            if (Solve(filled + 1)) return true;
            grid.ClearCell(x, y);
         }

         return false;
      }
   }
}
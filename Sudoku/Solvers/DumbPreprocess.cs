using System.Numerics;

namespace Sudoku.Solvers
{
   /// <summary>
   /// A sudoku solver that first preprocessess the board,
   /// removing all non-valid digits. The removed digits
   /// are then not tested in the Brute Force Solving part
   /// of the solver.
   /// The solver also uses an MVR approach where it
   /// tries the digits in order of the number of original
   /// valid digits. It goes without saying that this approach
   /// is very bad in the recursive method.
   /// </summary>
   public class DumpPreprocess : ISolvingAlgorithm
   {
      private Grid grid = null!;
      private const int BoardSidelength = 9;
      private int[,] ValidDigits = null!;

      public DumpPreprocess() { }
      public DumpPreprocess(Grid grid)
      {
         this.grid = grid;
      }

      public bool SolveGrid(Grid grid)
      {
         this.grid = grid;
         ValidDigits = new int[9, 9];
         for (int x = 0; x < BoardSidelength; x++)
            for (int y = 0; y < BoardSidelength; y++)
            {
               ValidDigits[x, y] = 0b1111111111111;
            }
         Preprocess();
         return Solve();
      }

      /// <summary>
      /// Repeatedly gets the valid digits for each individual cell and
      /// the number of valid digits for each cell, placing them in
      /// ValidDigits.
      /// Also saves the number of valid digits along with the digit
      /// mask.
      /// </summary>
      private void Preprocess()
      {
         bool progress;
         do
         {
            progress = false;
            for (int y = 0; y < BoardSidelength; y++)
            {
               for (int x = 0; x < BoardSidelength; x++)
               {
                  if (!grid.IsCellEmpty(x, y))
                  {
                     ValidDigits[x, y] = 0;
                     continue;
                  }

                  int avaliable = ~(grid.columns[x] | grid.rows[y] | grid.squares[(x / 3) + y / 3 * 3]) & 0x1FF;
                  int count = BitOperations.PopCount((uint)avaliable);

                  if (count == 0) return; // Unsolvable
                  if (count == 1)         // Set the digit
                  {
                     grid.SetCell(x, y, BitOperations.TrailingZeroCount(avaliable) + 1);
                     ValidDigits[x, y] = 0;
                     progress = true;
                     continue;
                  }

                  ValidDigits[x, y] = (count << 9) | avaliable;
               }
            }
         } while (progress);
      }

      /// <summary>
      /// Scans the board and returns the coordinates and mask of the empty cell
      /// with the fewest candidates. Returns (-1,-1,0) if no empties remain.
      /// </summary>
      private (int x, int y, int mask) FindMostConstrainedCell()
      {
         int bestX = -1, bestY = -1;
         int bestMask = 0;
         int minCount = BoardSidelength + 1;

         for (int y = 0; y < BoardSidelength; y++)
         {
            for (int x = 0; x < BoardSidelength; x++)
            {
               if (!grid.IsCellEmpty(x, y)) continue;

               int originalValid = ValidDigits[x, y];
               int mask = originalValid & 0b111111111;
               int count = originalValid >> 9;

               if (count == 0) return (x, y, 0);
               if (count == 1) return (x, y, mask);
               if (count < minCount)
               {
                  minCount = count;
                  bestMask = mask;
                  bestX = x;
                  bestY = y;
               }
            }
         }

         return (bestX, bestY, bestMask);
      }

      /// <summary>
      /// Recursive solver that picks the cell with the fewest original options.
      /// </summary>
      private bool Solve()
      {
         var (bestX, bestY, bestMask) = FindMostConstrainedCell();

         // If no empty cell found, the puzzle is solved
         if (bestX == -1) return true;

         // Try each candidate in the best cell
         int bits = bestMask;
         while (bits != 0)
         {
            int pick = bits & -bits;
            int digit = BitOperations.TrailingZeroCount(pick) + 1;
            bits &= bits - 1;

            if (!grid.IsValid(bestX, bestY, digit)) continue;

            grid.SetCell(bestX, bestY, digit);
            if (Solve()) return true;
            grid.ClearCell(bestX, bestY);
         }

         return false;
      }
   }
}
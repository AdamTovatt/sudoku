using System.Numerics;

namespace Sudoku.Solvers
{
   /// <summary>
   /// The MVRAlgorithm2 has more rules when finding the square
   /// that has the least possible options for digits. In addition
   /// to checking valid digits it also looks for 'hidden singles',
   /// which are digits that can be placed guaranteed.
   /// </summary>
   public class MVRAlgorithm2 : ISolvingAlgorithm
   {
      private const int BoardSidelength = 9;

      public MVRAlgorithm2() { }

      public bool SolveGrid(Grid grid)
      {
         return Solve(grid);
      }

      /// <summary>
      /// Scans the board and returns the coordinates and mask of the empty cell
      /// with the fewest candidates. Returns (-1,-1,0) if no empties remain.
      /// Includes tests for 'hidden singles'.
      /// </summary>
      private (int x, int y, int mask) FindMostConstrainedCell(Grid grid)
      {
         int bestX = -1, bestY = -1;
         int bestMask = 0;
         int minCount = BoardSidelength + 1;

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

                     if (!grid.IsCellEmpty(x, y)) continue;

                     int mask = ~(grid.columns[x] | grid.rows[y] | grid.squares[(x / 3) + y / 3 * 3]) & 0b111111111;
                     int count = BitOperations.PopCount((uint)mask);
                     if (count == 0) return (x, y, 0);
                     if (count == 1) return (x, y, mask);

                     int possibleMask = mask
                        & grid.columns[columnBoxStart + (dx + 1) % 3]
                        & grid.columns[columnBoxStart + (dx + 2) % 3]
                        & grid.rows[rowBoxStart + (dy + 1) % 3]
                        & grid.rows[rowBoxStart + (dy + 2) % 3];

                     if (BitOperations.PopCount((uint)possibleMask) == 1)
                        return (x, y, possibleMask);

                     if (count < minCount)
                     {
                        minCount = count;
                        bestMask = mask;
                        bestX = x;
                        bestY = y;
                     }
                  }
               }
            }
         }

         return (bestX, bestY, bestMask);
      }

      /// <summary>
      /// A recursive method that goes over all empty squares in the
      /// sudoku board, trying digits until a solution is found.
      /// </summary>
      /// <param name="grid"></param>
      /// <param name="filled"></param>
      /// <returns></returns>
      private bool Solve(Grid grid)
      {
         var (bestX, bestY, bestMask) = FindMostConstrainedCell(grid);

         // If no empty cell found, the puzzle is solved
         if (bestX == -1) return true;

         // Try each candidate in the best cell
         int bits = bestMask;
         while (bits != 0)
         {
            int pick = bits & -bits;
            int digit = BitOperations.TrailingZeroCount(pick) + 1;
            bits &= bits - 1;

            grid.SetCell(bestX, bestY, digit);
            if (Solve(grid)) return true;
            grid.ClearCell(bestX, bestY);
         }

         return false;
      }
   }
}
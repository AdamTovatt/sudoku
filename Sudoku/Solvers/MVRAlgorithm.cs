using System.Numerics;

namespace Sudoku.Solvers
{
   /// <summary>
   /// Essentially a brute force solver, except that it doesn't use
   /// IsValid() for each possible digit, but instead gets a bitmask
   /// with all possible valid digits for a given position. This in
   /// itself does not make the recursion all that much faster,
   /// since the engine is still quite 'dumb'.
   /// The main differentiating factor that makes it the MVR solver 
   /// faster is that within the recursive algorithm, it always 
   /// chooses the square with the least possibilities (given a
   /// set of rules) as the next square to search.
   /// </summary>
   public class MVRAlgorithm : ISolvingAlgorithm
   {
      private const int BoardSidelength = 9;

      public MVRAlgorithm() { }

      public bool SolveGrid(Grid grid)
      {
         return Solve(grid);
      }

      /// <summary>
      /// Scans the board and returns the coordinates and mask of the empty cell
      /// with the fewest candidates. Returns (-1,-1,0) if no empties remain.
      /// </summary>
      private (int x, int y, int mask) FindMostConstrainedCell(Grid grid)
      {
         int bestX = -1, bestY = -1;
         int bestMask = 0;
         int minCount = BoardSidelength + 1;

         for (int y = 0; y < BoardSidelength; y++)
         {
            for (int x = 0; x < BoardSidelength; x++)
            {
               if (!grid.IsCellEmpty(x, y)) continue;

               int mask = ~(grid.columns[x] | grid.rows[y] | grid.squares[(x / 3) + y / 3 * 3]) & 0b111111111;
               int count = BitOperations.PopCount((uint)mask);
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
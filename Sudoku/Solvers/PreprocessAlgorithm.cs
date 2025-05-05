using System.Numerics;
namespace Sudoku.Solvers
{
   /// <summary>
   /// A sudoku solver that first preprocessess the board,
   /// removing all non-valid digits. The removed digits
   /// are then not tested in the Brute Force Solving part
   /// of the solver.
   /// This is the solver that best fits the description
   /// of the "Rule 0 solver" from the project example solver.
   /// </summary>
   public class PreprocessAlgorithm : ISolvingAlgorithm
   {
      private const int BoardSidelength = 9;
      private int[,] ValidDigits = null!;

      public PreprocessAlgorithm() { }

      public bool SolveGrid(Grid grid)
      {
         ValidDigits = new int[9, 9];
         for (int x = 0; x < BoardSidelength; x++)
            for (int y = 0; y < BoardSidelength; y++)
            {
               ValidDigits[x, y] = 0b111111111;
            }
         Preprocess(grid);
         return Solve(grid, 0, 0);
      }

      /// <summary>
      /// Repeatedly gets the valid digits for each individual cell and
      /// the number of valid digits for each cell, placing them in
      /// ValidDigits.
      /// </summary>
      private void Preprocess(Grid grid)
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

                  ValidDigits[x, y] = avaliable;
               }
            }
         } while (progress);
      }

      /// <summary>
      /// Recursively solves the grid. Only tests digits that have been previously
      /// validated.
      /// </summary>
      private bool Solve(Grid grid, int x, int y)
      {
         if (x == 9) { x = 0; y++; }
         if (y == 9) return true;

         // If square is already filled
         if (grid.GetCell(x, y) != 0) return Solve(grid, x + 1, y);

         int avaliable = ValidDigits[x, y];

         for (int digit = 1; digit <= BoardSidelength; digit++)
         {
            if ((avaliable & (1 << (digit - 1))) == 0) continue;

            if (!grid.IsValid(x, y, digit)) continue;

            grid.SetCell(x, y, digit);
            if (Solve(grid, x + 1, y)) return true;
            grid.ClearCell(x, y);
         }

         return false;
      }
   }
}

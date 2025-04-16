namespace Sudoku
{
   public class BruteForceSolver : ISudokuSolver
   {
      public static IGrid Solve(string stringData)
      {
         IGrid grid = CleanerGrid.CreateFromString(stringData);

         if (!IsValidPuzzle(grid)) throw new ArgumentException("Grid has no solutions");

         Solve(grid, 0, 0);
         return grid;
      }

      public static IGrid Solve(IGrid grid)
      {
         Solve(grid, 0, 0);
         return grid;
      }

      private static bool Solve(IGrid grid, int row, int column)
      {
         if (column == grid.GetSideLength()) { column = 0; row++; }
         if (row == grid.GetSideLength()) return true;

         // If the digit is already filled in
         if (grid.GetCell(column, row) != 0) return Solve(grid, row, column + 1);

         // Tries new numbers if square is empty
         else
         {
            for (int digit = 1; digit <= 9; digit++)
            {
               if (grid.IsValid(column, row, digit))
               {
                  grid.SetCell(column, row, digit);
                  if (Solve(grid, row, column + 1)) return true;
                  grid.ClearCell(column, row); // Backtrack
               }
            }
         }

         return false;
      }

      public static bool IsValidPuzzle(IGrid grid)
      {
         throw new NotImplementedException();
      }
   }
}
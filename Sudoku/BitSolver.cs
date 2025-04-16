namespace Sudoku
{
   public class BitSolver : ISudokuSolver
   {
      public static IGrid Solve(string stringData)
      {
         IGrid grid = CleanerGrid.CreateFromString(stringData);

         if (!IsValidPuzzle(grid)) throw new ArgumentException("Grid has no solution.");

         throw new NotImplementedException();
      }

      public static bool IsValidPuzzle(IGrid grid)
      {
         throw new NotImplementedException();
      }
   }
}
namespace Sudoku
{
   public interface ISudokuSolver
   {
      /// <summary>
      /// Solves the Grid (Throws exception if not solvable?)
      /// </summary>
      /// <param name="stringData"></param>
      /// <returns></returns>
      static abstract IGrid Solve(string stringData);

      /// <summary>
      /// Return true if the grid is solvable
      /// </summary>
      /// <param name="puzzle"></param>
      /// <returns></returns>
      static abstract bool IsValidPuzzle(IGrid grid);
   }
}
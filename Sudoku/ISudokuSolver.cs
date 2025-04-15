namespace Sudoku
{
   public interface SudokuSolverInterface
   {
      /// <summary>
      /// Solves the Grid (Throws exception if not solvable?)
      /// </summary>
      /// <param name="stringData"></param>l
      /// <returns></returns>
      BitboardGrid Solve(string stringData);

      /// <summary>
      /// Return true if the grid is solvable
      /// </summary>
      /// <param name="puzzle"></param>
      /// <returns></returns>
      bool IsValidPuzzle(BitboardGrid grid);
   }
}
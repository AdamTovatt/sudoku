namespace Sudoku
{
   /// <summary>
   /// Represents a Sudoku grid. Like the "board" of a Sudoku.
   /// </summary>
   public interface IGrid
   {
      /// <summary>
      /// Will create an instance of <see cref="OriginalGrid"/> from the data in the provided string. Both "0" and "." work as empty tiles.
      /// </summary>
      /// <param name="gridString">The string with the data.</param>
      /// <param name="sideLength">Optional parameter for the length of a side in the square grid. Defaults to 9.</param>
      /// <returns>A new instance of <see cref="OriginalGrid"/></returns>
      static abstract IGrid CreateFromString(string gridString, int sideLength = 9);

      public int GetSideLength();

      /// <summary>
      /// Will return a bool indicating wether or not the grid that this method is called on is identical to the other grid in terms of the values that are in the cells of the grid.
      /// </summary>
      /// <param name="otherGrid">The other grid to compare to.</param>
      public bool HasSameCellValuesAs(IGrid otherGrid);

      /// <summary>
      /// Will get the value of a cell at a specific coordinate.
      /// </summary>
      /// <param name="x">The zero indexed x coordinate.</param>
      /// <param name="y">The zero indexed y coordinate.</param>
      /// <returns></returns>
      public int GetCell(int x, int y);

      /// <summary>
      /// Will set the value of a cell at a specific coordinate.
      /// </summary>
      /// <param name="x">The x coordinate to set the value for.</param>
      /// <param name="y">The y coordinate to set the value for.</param>
      /// <param name="value">The value to set at the given coordinates.</param>
      public void SetCell(int x, int y, int value);

      /// <summary>
      /// Will get a value indicating wether or not the cell at the provided coordinates is empty.
      /// </summary>
      /// <param name="x">The x coordinate to set the value for.</param>
      /// <param name="y">The y coordinate to set the value for.</param>
      public bool IsCellEmpty(int x, int y);

      public bool IsValid(int x, int y, int digit);

      public void ClearCell(int x, int y);
   }
}

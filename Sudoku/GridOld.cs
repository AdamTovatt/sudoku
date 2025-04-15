namespace Sudoku
{
    /// <summary>
    /// Represents a Sudoku grid. Like the "board" of a Sudoku.
    /// </summary>
    public class GridOld
    {
        private readonly int[,] grid; // this is where we store the numbers on the board

        /// <summary>
        /// The length of one of the sides of the grid. Assumes all grids are squares for now.
        /// </summary>
        public int SideLength { get; }

        /// <summary>
        /// Creates a new instance of <see cref="GridOld"/>
        /// </summary>
        /// <param name="sideLength">Optional parameter for the side length. Default is 9.</param>
        public GridOld(int sideLength = 9) // let's default the side length to 9 since we only care about that now anyway
        {
            SideLength = sideLength;
            grid = new int[sideLength, sideLength];
        }

        /// <summary>
        /// Will create an instance of <see cref="GridOld"/> from the data in the provided string. Both "0" and "." work as empty tiles.
        /// </summary>
        /// <param name="gridString">The string with the data.</param>
        /// <param name="sideLength">Optional parameter for the length of a side in the square grid. Defaults to 9.</param>
        /// <returns>A new instance of <see cref="GridOld"/></returns>
        public static GridOld CreateFromString(string gridString, int sideLength = 9)
        {
            // Check if the length of the gridString is valid
            if (gridString.Length != sideLength * sideLength)
                throw new ArgumentException("Grid string length does not match the expected grid size.");

            GridOld grid = new GridOld(sideLength);
            int index = 0;

            for (int y = 0; y < sideLength; y++)
            {
                for (int x = 0; x < sideLength; x++)
                {
                    char c = gridString[index];
                    index++;

                    if (c == '0' || c == '.') continue; // Skip empty cells

                    if (c >= '1' && c <= '0' + sideLength)
                    {
                        grid.SetCell(x, y, c - '0'); // Convert char digit to int
                    }
                    else
                    {
                        throw new ArgumentException($"Invalid character '{c}' at position {index - 1}.");
                    }
                }
            }

            return grid;
        }

        /// <summary>
        /// Will return a bool indicating wether or not the grid that this method is called on is identical to the other grid in terms of the values that are in the cells of the grid.
        /// </summary>
        /// <param name="otherGrid">The other grid to compare to.</param>
        public bool HasSameCellValuesAs(GridOld otherGrid)
        {
            // Check that they are the same size
            if (SideLength != otherGrid.SideLength) return false;

            // Match each element
            for (int y = 0; y < SideLength; y++)
            {
                for (int x = 0; x < SideLength; x++)
                {
                    if (GetCell(x, y) != otherGrid.GetCell(x, y)) return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Will get the value of a cell at a specific coordinate.
        /// </summary>
        /// <param name="x">The zero indexed x coordinate.</param>
        /// <param name="y">The zero indexed y coordinate.</param>
        /// <returns></returns>
        public int GetCell(int x, int y)
        {
            return grid[y, x];
        }

        /// <summary>
        /// Will set the value of a cell at a specific coordinate.
        /// </summary>
        /// <param name="x">The x coordinate to set the value for.</param>
        /// <param name="y">The y coordinate to set the value for.</param>
        /// <param name="value">The value to set at the given coordinates.</param>
        public void SetCell(int x, int y, int value)
        {
            if (value < 0 || value > SideLength)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"Value must be between 0 and 9 (0 means empty).");
            }

            if (x < 0 || x >= SideLength)
                throw new ArgumentOutOfRangeException(nameof(x), $"The index \"{x}\" is not valid. Must be between 0 and {SideLength}.");

            if (y < 0 || y >= SideLength)
                throw new ArgumentOutOfRangeException(nameof(y), $"The index \"{y}\" is not valid. Must be between 0 and {SideLength}.");

            grid[y, x] = value;
        }

        /// <summary>
        /// Will get a value indicating wether or not the cell at the provided coordinates is empty.
        /// </summary>
        /// <param name="x">The x coordinate to set the value for.</param>
        /// <param name="y">The y coordinate to set the value for.</param>
        public bool IsCellEmpty(int x, int y)
        {
            return grid[y, x] == 0;
        }
    }
}

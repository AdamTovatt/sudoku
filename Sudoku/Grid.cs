namespace Sudoku
{
    public class Grid
    {
        private readonly int[,] grid;
        public int SideLength { get; }
        public int BlockWidth { get; }
        public int BlockHeight { get; }

        public Grid(int blockWidth, int blockHeight)
        {
            BlockWidth = blockWidth;
            BlockHeight = blockHeight;
            SideLength = blockWidth * blockHeight;
            grid = new int[SideLength, SideLength];
        }

        public static Grid CreateFromString(string gridString)
        {
            throw new NotImplementedException($"{nameof(CreateFromString)} needs to be implemented before it can be used!");
        }

        public bool HasSameCellValuesAs(Grid otherGrid)
        {
            throw new NotImplementedException($"{nameof(CreateFromString)} needs to be implemented before it can be used!");
        }

        public int GetCell(int x, int y)
        {
            return grid[y, x];
        }

        public void SetCell(int x, int y, int value)
        {
            if (value < 0 || value > SideLength)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"Value must be between 0 and {SideLength} (0 means empty).");
            }

            grid[y, x] = value;
        }

        public bool IsCellEmpty(int x, int y)
        {
            return grid[y, x] == 0;
        }

        public void Clear()
        {
            for (int y = 0; y < SideLength; y++)
            {
                for (int x = 0; x < SideLength; x++)
                {
                    grid[y, x] = 0;
                }
            }
        }

        public static bool TryGetBlockDimensions(string input, out int blockWidth, out int blockHeight)
        {
            int length = input.Length;
            int sideLength = 1;
            int square = 1;

            while (square < length)
            {
                sideLength++;
                square = sideLength * sideLength;
            }

            if (square != length)
            {
                blockWidth = 0;
                blockHeight = 0;
                return false;
            }

            for (int w = 1; w <= sideLength; w++)
            {
                if (sideLength % w == 0)
                {
                    blockWidth = w;
                    blockHeight = sideLength / w;
                    return true;
                }
            }

            blockWidth = 0;
            blockHeight = 0;
            return false;
        }
    }
}

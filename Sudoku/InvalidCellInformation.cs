namespace Sudoku
{
    public class InvalidCellInformation
    {
        public int X { get; init; }
        public int Y { get; init; }
        public int Value { get; init; }

        public InvalidCellInformation(int x, int y, int value)
        {
            X = x;
            Y = y;
            Value = value;
        }

        public override string ToString()
        {
            return $"Invalid cell value at ({X}, {Y}): {Value}";
        }
    }
}

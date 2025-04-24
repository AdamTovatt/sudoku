using Sudoku.Data.Models;

namespace Sudoku.Data.Providers
{
    public class CsvStreamPuzzleProvider : ISudokuPuzzleProvider
    {
        private Stream puzzleStream;

        private CsvStreamPuzzleProvider(Stream puzzleStream)
        {
            this.puzzleStream = puzzleStream;
        }

        public static CsvStreamPuzzleProvider CreateWithStream(Stream puzzleStream)
        {
            return new CsvStreamPuzzleProvider(puzzleStream);
        }

        public SudokuPuzzle? GetNext(PuzzleDifficulty difficulty = PuzzleDifficulty.Unspecified)
        {
            SudokuPuzzle? result = GetNext();

            while (difficulty != PuzzleDifficulty.Unspecified && result != null && result.Difficulty != difficulty)
            {
                result = GetNext();
            }

            return result;
        }

        private SudokuPuzzle? GetNext()
        {
            List<byte> buffer = new List<byte>();
            int b;

            if (puzzleStream.Position == 0)
            {
                // First, just read the header without using saving it, we don't need it
                while ((b = puzzleStream.ReadByte()) != -1)
                {
                    if (b == '\n') break;
                }
            }

            // Now we should've read the header, let's read an actual row
            while ((b = puzzleStream.ReadByte()) != -1)
            {
                if (b == '\n') break; // break on new line character
                if (b != '\r') buffer.Add((byte)b); // add the byte that we read unless it's a return character
            }

            if (buffer.Count == 0) return null;

            string line = System.Text.Encoding.UTF8.GetString(buffer.ToArray());
            return SudokuPuzzle.FromString(line);
        }

        public void Dispose()
        {
            puzzleStream.Dispose();
        }
    }
}

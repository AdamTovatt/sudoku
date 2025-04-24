using Sudoku.Data.Models;
using Sudoku.Resources;

namespace Sudoku.Data.Providers
{
    public class EmbeddedResourcesCsvStreamPuzzleProvider : ISudokuPuzzleProvider
    {
        private CsvStreamPuzzleProvider easyStream;
        private CsvStreamPuzzleProvider mediumStream;
        private CsvStreamPuzzleProvider hardStream;
        private CsvStreamPuzzleProvider expertStream;

        private Random random;

        private EmbeddedResourcesCsvStreamPuzzleProvider(CsvStreamPuzzleProvider easyStream, CsvStreamPuzzleProvider mediumStream, CsvStreamPuzzleProvider hardStream, CsvStreamPuzzleProvider expertStream)
        {
            this.easyStream = easyStream;
            this.mediumStream = mediumStream;
            this.hardStream = hardStream;
            this.expertStream = expertStream;

            random = new Random();
        }

        public static EmbeddedResourcesCsvStreamPuzzleProvider Create()
        {
            CsvStreamPuzzleProvider easy = CsvStreamPuzzleProvider.CreateWithStream(ResourceHelper.Instance.GetFileStream(Resource.SudokuPuzzles.Easy));
            CsvStreamPuzzleProvider medium = CsvStreamPuzzleProvider.CreateWithStream(ResourceHelper.Instance.GetFileStream(Resource.SudokuPuzzles.Medium));
            CsvStreamPuzzleProvider hard = CsvStreamPuzzleProvider.CreateWithStream(ResourceHelper.Instance.GetFileStream(Resource.SudokuPuzzles.Hard));
            CsvStreamPuzzleProvider expert = CsvStreamPuzzleProvider.CreateWithStream(ResourceHelper.Instance.GetFileStream(Resource.SudokuPuzzles.Expert));

            return new EmbeddedResourcesCsvStreamPuzzleProvider(easy, medium, hard, expert);
        }

        public SudokuPuzzle? GetNext(PuzzleDifficulty difficulty = PuzzleDifficulty.Unspecified)
        {
            switch (difficulty)
            {
                case PuzzleDifficulty.Unspecified:
                    switch (random.Next(0, 4))
                    {
                        case 0:
                            return easyStream.GetNext();
                        case 1:
                            return mediumStream.GetNext();
                        case 2:
                            return hardStream.GetNext();
                        case 3:
                            return expertStream.GetNext();
                        default:
                            throw new InvalidOperationException("Invalid value when chosing difficulty at random.");
                    }
                case PuzzleDifficulty.Easy:
                    return easyStream.GetNext();
                case PuzzleDifficulty.Medium:
                    return mediumStream.GetNext();
                case PuzzleDifficulty.Hard:
                    return hardStream.GetNext();
                case PuzzleDifficulty.Expert:
                    return expertStream.GetNext();
                default: throw new ArgumentException($"Unknown difficulty specified: {difficulty}");
            }
        }

        public void Dispose()
        {
            easyStream.Dispose();
            mediumStream.Dispose();
            hardStream.Dispose();
            expertStream.Dispose();
        }
    }
}

using Sudoku.Data.Models;

namespace Sudoku.Data.Providers
{
    public class InMemoryPuzzleProvider : ISudokuPuzzleProvider
    {
        private List<SudokuPuzzle> puzzles;
        private int currentIndex = 0;

        public InMemoryPuzzleProvider()
        {
            puzzles = new List<SudokuPuzzle>();
        }

        public void AddPuzzle(SudokuPuzzle puzzle)
        {
            puzzles.Add(puzzle);
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
            if (currentIndex < puzzles.Count)
            {
                SudokuPuzzle result = puzzles[currentIndex];
                currentIndex++;
                return result;
            }

            return null;
        }

        public void Dispose()
        {
            return; // nothing to dispose for this implementation
        }
    }
}

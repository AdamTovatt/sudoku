namespace Sudoku.Data
{
    public interface ISudokuPuzzleProvider
    {
        public SudokuPuzzle GetNext(PuzzleDifficulty puzzleDifficulty = PuzzleDifficulty.Unspecified);
    }
}

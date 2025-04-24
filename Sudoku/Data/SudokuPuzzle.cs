namespace Sudoku.Data
{
    public class SudokuPuzzle
    {
        public Grid StartingGrid { get; set; }
        public Grid SolvedGrid { get; set; }
        public PuzzleDifficulty PuzzleDifficulty { get; set; }
        public int Clues { get; set; }

        public SudokuPuzzle(Grid startingGrid, Grid solvedGrid, PuzzleDifficulty puzzleDifficulty, int clues)
        {
            StartingGrid = startingGrid;
            SolvedGrid = solvedGrid;
            PuzzleDifficulty = puzzleDifficulty;
            Clues = clues;
        }
    }
}

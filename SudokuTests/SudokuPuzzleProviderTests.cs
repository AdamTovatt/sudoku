using Sudoku.Data.Models;
using Sudoku.Data.Providers;

namespace SudokuTests
{
    [TestClass]
    public class SudokuPuzzleProviderTests
    {
        [TestMethod]
        public void GetFromEmbeddedResource()
        {
            using (ISudokuPuzzleProvider provider = EmbeddedResourcesCsvStreamPuzzleProvider.Create())
            {
                SudokuPuzzle? easyPuzzle = provider.GetNext(PuzzleDifficulty.Easy);
                Assert.IsNotNull(easyPuzzle);
                Assert.AreEqual(PuzzleDifficulty.Easy, easyPuzzle.Difficulty);

                SudokuPuzzle? mediumPuzzle = provider.GetNext(PuzzleDifficulty.Medium);
                Assert.IsNotNull(mediumPuzzle);
                Assert.AreEqual(PuzzleDifficulty.Medium, mediumPuzzle.Difficulty);

                SudokuPuzzle? hardPuzzle = provider.GetNext(PuzzleDifficulty.Hard);
                Assert.IsNotNull(hardPuzzle);
                Assert.AreEqual(PuzzleDifficulty.Hard, hardPuzzle.Difficulty);

                SudokuPuzzle? expertPuzzle = provider.GetNext(PuzzleDifficulty.Expert);
                Assert.IsNotNull(expertPuzzle);
                Assert.AreEqual(PuzzleDifficulty.Expert, expertPuzzle.Difficulty);

                SudokuPuzzle? first = provider.GetNext();
                Assert.IsNotNull(first);

                SudokuPuzzle? second;
                do
                {
                    second = provider.GetNext();
                }
                while (second != null && second.Difficulty == first!.Difficulty);

                Assert.IsNotNull(second);
                Assert.AreNotEqual(first!.Difficulty, second!.Difficulty);
            }
        }
    }
}

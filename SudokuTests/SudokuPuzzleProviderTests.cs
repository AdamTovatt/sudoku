using Sudoku.Data.Models;
using Sudoku.Data.Providers;

namespace SudokuTests
{
    [TestClass]
    public class SudokuPuzzleProviderTests
    {
        [TestMethod]
        public void GetFromCsvPuzzleProvider()
        {
            using (ISudokuPuzzleProvider provider = CsvStreamPuzzleProvider.CreateWithStream(File.OpenRead("C:\\users\\adam\\desktop\\sudoku-3m.csv")))
            {
                Dictionary<PuzzleDifficulty, List<SudokuPuzzle>> puzzlesByDifficulty = new Dictionary<PuzzleDifficulty, List<SudokuPuzzle>>();
                SudokuPuzzle? currentPuzzle;

                do
                {
                    currentPuzzle = provider.GetNext();

                    if (currentPuzzle != null)
                    {
                        if (!puzzlesByDifficulty.TryGetValue(currentPuzzle.Difficulty, out List<SudokuPuzzle>? list))
                        {
                            list = new List<SudokuPuzzle>();
                            puzzlesByDifficulty[currentPuzzle.Difficulty] = list;
                        }

                        if (list.Count < 50000)
                        {
                            list.Add(currentPuzzle);
                        }

                        if (puzzlesByDifficulty.All(x => x.Value.Count == 50000))
                            break;
                    }
                }
                while (currentPuzzle != null);

                foreach (KeyValuePair<PuzzleDifficulty, List<SudokuPuzzle>> keyValuePair in puzzlesByDifficulty)
                {
                    string fileName = $"sudoku-9x9-{keyValuePair.Key.ToString().ToLower()}.csv";

                    using StreamWriter writer = new StreamWriter(fileName);
                    writer.WriteLine(SudokuPuzzle.GetCsvHeader());

                    foreach (SudokuPuzzle puzzle in keyValuePair.Value)
                    {
                        writer.WriteLine(puzzle.ToString());
                    }
                }
            }
        }
    }
}

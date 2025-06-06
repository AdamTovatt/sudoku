using Sudoku;
using Sudoku.Solvers;

namespace SudokuTests
{
    [TestClass]
    public class GridTests
    {
        [TestMethod]
        public void CreateEmptyCorrectly()
        {
            int xSideLength = 9;
            int ySideLength = 9;

            string zeros = new string('0', xSideLength * ySideLength);

            Grid grid = Grid.CreateFromString(zeros);

            for (int x = 0; x < xSideLength; x++)
            {
                for (int y = 0; y < ySideLength; y++)
                {
                    Assert.IsTrue(grid.IsCellEmpty(0, 0));
                }
            }
        }

        [TestMethod]
        public void GetAndSetValues()
        {
            int xSideLength = 9;
            int ySideLength = 9;

            string zeros = new string('0', xSideLength * ySideLength);

            Grid grid = Grid.CreateFromString(zeros);

            grid.SetCell(x: 0, y: 0, value: 4);
            Assert.IsFalse(grid.IsCellEmpty(0, 0));
            Assert.AreEqual(4, grid.GetCell(0, 0));

            grid.SetCell(x: 4, y: 7, value: 8);
            Assert.IsFalse(grid.IsCellEmpty(4, 7));
            Assert.AreEqual(8, grid.GetCell(4, 7));
        }

        [TestMethod]
        public void CreateGridFromString1()
        {
            const string stringData = "1..5.37..6.3..8.9......98...1.......8761..........6...........7.8.9.76.47...6.312";

            Grid grid = Grid.CreateFromString(stringData, 9);

            Assert.AreEqual(9, grid.SideLength);

            Assert.IsFalse(grid.IsCellEmpty(0, 0));
            Assert.IsTrue(grid.IsCellEmpty(1, 0));

            Assert.AreEqual(1, grid.GetCell(0, 0));
            Assert.AreEqual(0, grid.GetCell(1, 0));
            Assert.AreEqual(0, grid.GetCell(2, 0));
            Assert.AreEqual(5, grid.GetCell(3, 0));
            Assert.AreEqual(0, grid.GetCell(4, 0));
            Assert.AreEqual(3, grid.GetCell(5, 0));
            Assert.AreEqual(7, grid.GetCell(6, 0));
            Assert.AreEqual(0, grid.GetCell(7, 0));
            Assert.AreEqual(0, grid.GetCell(8, 0));

            Assert.AreEqual(6, grid.GetCell(0, 1));
            Assert.AreEqual(0, grid.GetCell(1, 1));
            Assert.AreEqual(3, grid.GetCell(2, 1));
            Assert.AreEqual(0, grid.GetCell(3, 1));
            Assert.AreEqual(0, grid.GetCell(4, 1));
            Assert.AreEqual(8, grid.GetCell(5, 1));
            Assert.AreEqual(0, grid.GetCell(6, 1));
            Assert.AreEqual(9, grid.GetCell(7, 1));
            Assert.AreEqual(0, grid.GetCell(8, 1));

            Assert.AreEqual(0, grid.GetCell(0, 2));
            Assert.AreEqual(0, grid.GetCell(1, 2));
            Assert.AreEqual(0, grid.GetCell(2, 2));

            Assert.AreEqual(9, grid.GetCell(5, 2));
        }

        [TestMethod]
        public void CreateGridFromString2()
        {
            const string stringData = "..3.8.5...4.....9.7..15.4..98...1..5..2.7983...5.6....1....6.5.8.....7..........4";

            Grid grid = Grid.CreateFromString(stringData, 9);

            Assert.AreEqual(9, grid.SideLength);

            Assert.IsTrue(grid.IsCellEmpty(0, 0));
            Assert.IsTrue(grid.IsCellEmpty(1, 0));
            Assert.AreEqual(3, grid.GetCell(2, 0));
            Assert.AreEqual(8, grid.GetCell(4, 0));
            Assert.AreEqual(5, grid.GetCell(6, 0));

            Assert.AreEqual(4, grid.GetCell(1, 1));
            Assert.IsTrue(grid.IsCellEmpty(0, 1));
            Assert.IsTrue(grid.IsCellEmpty(2, 1));
            Assert.IsTrue(grid.IsCellEmpty(3, 1));
            Assert.IsTrue(grid.IsCellEmpty(4, 1));
            Assert.AreEqual(9, grid.GetCell(0, 3));
            Assert.AreEqual(7, grid.GetCell(4, 4));

            Assert.AreEqual(1, grid.GetCell(3, 2));
            Assert.AreEqual(5, grid.GetCell(6, 0));
        }

        [DataTestMethod]
        [DataRow(".9.....8.5......96......4..6..34....9......2.2...6..17.1....8...6..17..97...95...", ".9.....8.5......96......4..6..34....9......2.2...6..17.1....8...6..17..97...95...", true)]
        [DataRow("..7.....34...6..12.....37..1.8.57.......8..6......21...........6...459....9....87", "..7.....34...6..12.....37..1.8.57.......8..6......21...........6...459....9....87", true)]
        [DataRow(".9.....8.5......96......4..6..34....9......2.2...6..17.1....8...6..17..97...95...", "..7.....34...6..12.....37..1.8.57.......8..6......21...........6...459....9....87", false)]
        [DataRow(".9.....8.5......96......4..6..34....9......2.2...6..17.1....8...6..17..97...95...", ".9.....8.5......96......4..6..34....91.....2.2...6..17.1....8...6..17..97...95...", false)]
        public void CompareGrids(string gridData1, string gridData2, bool expectedResult)
        {
            Grid grid1 = Grid.CreateFromString(gridData1, 9);
            Grid grid2 = Grid.CreateFromString(gridData2, 9);
            Assert.AreEqual(expectedResult, grid1.HasSameCellValuesAs(grid2));

            grid1 = Grid.CreateFromString(gridData1);
            grid2 = Grid.CreateFromString(gridData2);
            Assert.AreEqual(expectedResult, grid1.HasSameCellValuesAs(grid2));
        }

        [TestMethod]
        public void GetIsValid()
        {
            const string stringData = ".................................................................................";

            Grid grid = Grid.CreateFromString(stringData);

            grid.SetCell(0, 0, 1);

            Assert.IsFalse(grid.IsValid(2, 2, 1)); // False by square
            Assert.IsFalse(grid.IsValid(0, 8, 1)); // False by column
            Assert.IsFalse(grid.IsValid(5, 0, 1)); // False by row

            Assert.IsTrue(grid.IsValid(1, 2, 8));
            Assert.IsTrue(grid.IsValid(7, 1, 3));
            Assert.IsTrue(grid.IsValid(8, 6, 2));

            Assert.IsTrue(grid.IsValid(1, 2, 8));
            Assert.IsTrue(grid.IsValid(7, 1, 3));
            Assert.IsTrue(grid.IsValid(8, 6, 2));

            grid.SetCell(0, 0, 4);

            Assert.IsFalse(grid.IsValid(2, 2, 4)); // False by square
            Assert.IsFalse(grid.IsValid(0, 8, 4)); // False by column
            Assert.IsFalse(grid.IsValid(5, 0, 4)); // False by row
        }
    }
}
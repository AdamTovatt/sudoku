using Sudoku;

namespace SudokuTests
{
    [TestClass]
    public class GridTests
    {
        public TestContext TestContext { get; set; }

        public IGrid CreateGrid(string stringData, int sideLength = 9)
        {
            return CleanerGrid.CreateFromString(stringData, sideLength);
        }

        [TestMethod]
        public void TestEmpty()
        {
            string zeros = new string('0', 81);

            IGrid grid = BitboardGrid.CreateFromString(zeros);

            Assert.IsTrue(grid.IsCellEmpty(0, 0));

            grid.SetCell(0, 0, 4);

            Assert.IsFalse(grid.IsCellEmpty(0, 0));
        }

        public void PrintBoard(IGrid grid)
        {
            TestContext.WriteLine(grid.ToString());
        }

        [TestMethod]
        public void CreateGridFromString1()
        {
            const string stringData = "1..5.37..6.3..8.9......98...1.......8761..........6...........7.8.9.76.47...6.312";

            IGrid grid = CreateGrid(stringData, 9);

            Assert.AreEqual(9, grid.GetSideLength());

            PrintBoard(grid);
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

            IGrid grid = CreateGrid(stringData, 9);

            Assert.AreEqual(9, grid.GetSideLength());

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

        [TestMethod]
        public void CompareTwoGrids1()
        {
            const string gridData = ".9.....8.5......96......4..6..34....9......2.2...6..17.1....8...6..17..97...95...";

            IGrid grid1 = CreateGrid(gridData, 9);
            IGrid grid2 = CreateGrid(gridData, 9);

            Assert.IsTrue(grid1.HasSameCellValuesAs(grid2));

            grid1 = CreateGrid(gridData, 9);
            grid2 = CreateGrid(gridData); // create with out optional parameter here just to ensure the default of 9 is still working

            Assert.IsTrue(grid1.HasSameCellValuesAs(grid2));
        }

        [TestMethod]
        public void CompareTwoGrids2()
        {
            const string gridData = "..7.....34...6..12.....37..1.8.57.......8..6......21...........6...459....9....87";

            IGrid grid1 = CreateGrid(gridData, 9);
            IGrid grid2 = CreateGrid(gridData, 9);

            Assert.IsTrue(grid1.HasSameCellValuesAs(grid2));

            grid1 = CreateGrid(gridData, 9);
            grid2 = CreateGrid(gridData); // create with out optional parameter here just to ensure the default of 9 is still working

            Assert.IsTrue(grid1.HasSameCellValuesAs(grid2));
        }

        [TestMethod]
        public void CompareTwoGrids3()
        {
            const string gridData1 = ".9.....8.5......96......4..6..34....9......2.2...6..17.1....8...6..17..97...95...";
            const string gridData2 = "..7.....34...6..12.....37..1.8.57.......8..6......21...........6...459....9....87";

            IGrid grid1 = CreateGrid(gridData1, 9);
            IGrid grid2 = CreateGrid(gridData2, 9);

            Assert.IsFalse(grid1.HasSameCellValuesAs(grid2));

            grid1 = CreateGrid(gridData1, 9);
            grid2 = CreateGrid(gridData2); // create with out optional parameter here just to ensure the default of 9 is still working

            Assert.IsFalse(grid1.HasSameCellValuesAs(grid2));
        }

        [TestMethod]
        public void CompareTwoGrids4()
        {
            const string gridData1 = ".9.....8.5......96......4..6..34....9......2.2...6..17.1....8...6..17..97...95...";
            const string gridData2 = ".9.....8.5......96......4..6..34....91.....2.2...6..17.1....8...6..17..97...95..."; // just one character differing

            IGrid grid1 = CreateGrid(gridData1, 9);
            IGrid grid2 = CreateGrid(gridData2, 9);

            Assert.IsFalse(grid1.HasSameCellValuesAs(grid2));

            grid1 = CreateGrid(gridData1, 9);
            grid2 = CreateGrid(gridData2); // create with out optional parameter here just to ensure the default of 9 is still working

            Assert.IsFalse(grid1.HasSameCellValuesAs(grid2));
        }
    }
}
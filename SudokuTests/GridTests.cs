using Sudoku;

namespace SudokuTests
{
    [TestClass]
    public class GridTests
    {
        [TestMethod]
        public void GetSideLengthFromString1()
        {
            // 9x9 string
            const string gridString = "900000002010060390083900100804095007130670049060041000302010050000500000541080030";

            bool couldGetSideLength = Grid.TryGetBlockDimensions(gridString, out int blockWidth, out int blockHeight);

            Assert.IsTrue(couldGetSideLength);

            // 9x9 grids are created by 3x3 grids of 3x3 blocks, so the blocks should have side lengths that are 3
            Assert.AreEqual(3, blockWidth);
            Assert.AreEqual(3, blockHeight);
        }

        [TestMethod]
        public void CreateGridFromString1()
        {
            const string gridString = "900000002010060390083900100804095007130670049060041000302010050000500000541080030";

            Grid grid = Grid.CreateFromString(gridString);
        }
    }
}
using Sudoku.Data.Providers;
using Sudoku.Resources;
using System.Reflection;

namespace SudokuTests
{
    [TestClass]
    public class AssemblyInitializer
    {
        [AssemblyInitialize]
        public static void Initialize(TestContext context)
        {
            ResourceHelper.Initialize(Assembly.GetAssembly(typeof(EmbeddedResourcesCsvStreamPuzzleProvider)));
        }
    }
}

using CommandLine;

namespace SudokuCli.Cli
{
    public class RawInputArguments
    {
        [Option('a', "algorithm", Required = true, MetaValue = "ALGORITHM", HelpText = "The solving algorithm to use for solving Sudoku puzzles with. Case insensitive, meaning all versions of an algorithm name works. Doesn't matter which characters in the name are upper case, if any.")]
        public AlgorithmOption? AlgorithmOption { get; set; }

        [Option('c', "count", Required = true, Default = 100, MetaValue = "COUNT", HelpText = "The amount of Sudoku puzzles to solve.")]
        public int Count { get; set; }

        [Option('o', "output", Required = false, Default = "", MetaValue = "OUTPUT", HelpText = "The output directory to create the output files in. Output files will override existing ones that were created from the same parameters. The default value is just an empty string which will result in a path relative to the executing application, meaning the output will be in the same directory as the executing file is in.")]
        public string? OutputBasePath { get; set; }
    }
}

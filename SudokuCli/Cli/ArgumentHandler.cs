using CommandLine;
using CommandLine.Text;
using Sudoku.Solvers;
using Sudoku;
using System.Text;
using Sudoku.Data.Models;

namespace SudokuCli.Cli
{
    public class ArgumentHandler
    {
        public static InputArguments? GetInputArguments(string[] args)
        {
            Parser parser = new Parser(settings => settings.HelpWriter = null);
            ParserResult<RawInputArguments> parseResult = parser.ParseArguments<RawInputArguments>(args);

            if (parseResult.Errors.Any())
            {
                DisplayHelp(parseResult, parseResult.Errors);
                return null;
            }
            else
            {
                return GetInputArgumentsInternal(parseResult);
            }
        }

        private static InputArguments? GetInputArgumentsInternal(ParserResult<RawInputArguments> result)
        {
            RawInputArguments options = result.Value;

            if (options.AlgorithmOption == null)
            {
                DisplayError(result, "Missing algorithm option.");
                return null;
            }

            if (options.AlgorithmOption.GetTypeOfAlgorithm() == null)
            {
                DisplayError(result, $"Invalid algorithm option provided, no algorithm with provided name could be found: \"{options.AlgorithmOption.Name}\"");
                return null;
            }

            if (options.OutputBasePath == null)
            {
                DisplayError(result, $"Missing output base path.");
                return null;
            }

            if (options.Count < 0 || options.Count > 50000)
            {
                DisplayError(result, $"Invalid count value. Min value is 1. Max value is 50 000. Test data doesn't contain more than 50 000 puzzles.");
                return null;
            }

            if (options.DifficultyOption == null || options.DifficultyOption.GetDifficulty() == null)
            {
                DisplayError(result, $"Invalid difficulty option provided: \"{options.DifficultyOption?.Name}\"");
                return null;
            }

            ISolvingAlgorithm solvingAlgorithm = options.AlgorithmOption.CreateAlgorithmInstance();
            PuzzleDifficulty? difficulty = options.DifficultyOption.GetDifficulty();

            if (difficulty == null)
            {
                DisplayError(result, $"Invalid difficulty option provided: \"{options.DifficultyOption?.Name}\"");
                return null;
            }

            return new InputArguments(solvingAlgorithm, options.Count, options.OutputBasePath, difficulty.Value);
        }

        private static void DisplayError<T>(ParserResult<T> result, string errorMessage)
        {
            Console.WriteLine($"ERROR(S): {errorMessage}\n");

            try
            {
                HelpText helpText = HelpText.AutoBuild(result);
                Console.WriteLine(helpText);
            }
            catch { }
        }

        private static void DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errors)
        {
            HelpText helpText = HelpText.AutoBuild(result, h =>
            {
                h.Copyright = string.Empty;
                h.AddPostOptionsLine(GetAvailableAlgorithmsList());
                return HelpText.DefaultParsingErrorsHandler(result, h);
            }, e => e);
            Console.WriteLine(helpText);
        }

        private static string GetAvailableAlgorithmsList()
        {
            List<Type> algorithmTypes = Solver.GetAvailableSolvers();

            StringBuilder result = new StringBuilder("------- Available Algorithms ----------\r\n");

            foreach (Type type in algorithmTypes)
            {
                result.AppendLine($"* {type.Name}");
            }

            return result.ToString();
        }
    }
}

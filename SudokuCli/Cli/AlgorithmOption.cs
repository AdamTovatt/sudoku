using Sudoku;
using Sudoku.Solvers;

namespace SudokuCli.Cli
{
    public class AlgorithmOption
    {
        public string Name { get; set; }

        public AlgorithmOption(string name)
        {
            Name = name;
        }

        public Type? GetTypeOfAlgorithm()
        {
            return Solver.GetAvailableSolvers().Find(x => x.Name.ToLower() == Name.ToLower());
        }

        public ISolvingAlgorithm CreateAlgorithmInstance()
        {
            Type? algorithmType = GetTypeOfAlgorithm();

            if (algorithmType == null)
                throw new InvalidOperationException($"No algorithm with name \"{Name}\" could be found.");

            object? createdInstance = Activator.CreateInstance(algorithmType);

            if (createdInstance == null)
                throw new Exception($"Unknown error when trying to create instance of algorithm with name {Name}.");

            return (ISolvingAlgorithm)createdInstance;
        }
    }
}

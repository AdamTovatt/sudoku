#!/usr/bin/env bash

# Variables
Rdir="./RData/R/"
BFdir="./RData/data/raw-data/BruteForceAlgorithm/"
MVR1dir="./RData/data/raw-data/MVRAlgorithm/"
MVR2dir="./RData/data/raw-data/MVRAlgorithm2/"

# Create dirs
createDirs () {
  mkdir -p ./RData/data/raw-data/{BruteForceAlgorithm,MVRAlgorithm,MVRAlgorithm2}
  mkdir -p ./RData/data/graphs
  echo "Created necessary directories."
}

# Change permissions
changePerms () {
  if chmod +x ${Rdir}Sudoku.R; then
  	echo "Changed permissions."
  else
  	echo "Permission change for SudokuR.R failed"
  fi
}

readIterations () {
  echo "You must choose how many times you want algorithms to be run:"

  read -p "BruteforceAlgorithm: " BFits
  while ! [[ "$BFits" =~ ^[0-9]+$ ]]; do
      echo "Please enter a valid number."
      read -p "BruteforceAlgorithm: " BFits
  done

  read -p "MVRAlgorithm: " MVR1its
  while ! [[ "$MVR1its" =~ ^[0-9]+$ ]]; do
      echo "Please enter a valid number."
      read -p "BruteforceAlgorithm: " MVR1its
  done

  read -p "MVRAlgorithm2: " MVR2its
  while ! [[ "$MVR2its" =~ ^[0-9]+$ ]]; do
      echo "Please enter a valid number."
      read -p "BruteforceAlgorithm: " MVR2its
  done

  export BFits MVR1its MVR2its
}

# Get the data!!!
runSudoku() {
	dotnet run --project ./SudokuCli/SudokuCli.csproj -- --algorithm $1 --difficulty $2 --count $3 --output $4
}

runAllSudoku() {
  runSudoku BruteForceAlgorithm Easy "$BFits" $BFdir && echo "BruteforceAlgorithm Easy Success!" || echo "Failed (exit code: $?)."
  runSudoku BruteForceAlgorithm Medium "$BFits" $BFdir && echo "BruteforceAlgorithm Medium Success!" || echo "Failed (exit code: $?)."
  runSudoku BruteForceAlgorithm Hard "$BFits" $BFdir && echo "BruteforceAlgorithm Hard Success!" || echo "Failed (exit code: $?)."
  runSudoku BruteForceAlgorithm Expert "$BFits" $BFdir && echo "BruteforceAlgorithm Expert Success!" || echo "Failed (exit code: $?)."

  runSudoku MVRAlgorithm Easy "$MVR1its" $MVR1dir && echo "MVRAlgorithm Easy Success!" || echo "Failed (exit code: $?)."
  runSudoku MVRAlgorithm Medium "$MVR1its" $MVR1dir && echo "MVRAlgorithm Medium Success!" || echo "Failed (exit code: $?)."
  runSudoku MVRAlgorithm Hard "$MVR1its" $MVR1dir && echo "MVRAlgorithm Hard Success!" || echo "Failed (exit code: $?)."
  runSudoku MVRAlgorithm Expert "$MVR1its" $MVR1dir && echo "MVRAlgorithm Expert Success!" || echo "Failed (exit code: $?)."

  runSudoku MVRAlgorithm2 Easy "$MVR2its" $MVR2dir && echo "MVRAlgorithm2 Easy Success!" || echo "Failed (exit code: $?)."
  runSudoku MVRAlgorithm2 Medium "$MVR2its" $MVR2dir && echo "MVRAlgorithm2 Medium Success!" || echo "Failed (exit code: $?)."
  runSudoku MVRAlgorithm2 Hard "$MVR2its" $MVR2dir && echo "MVRAlgorithm2 Hard Success!" || echo "Failed (exit code: $?)."
  runSudoku MVRAlgorithm2 Expert "$MVR2its" $MVR2dir && echo "MVRAlgorithm2 Expert Success!" || echo "Failed (exit code: $?)."
}


echo "Choose an option:"
echo "1. Only get the data"
echo "2. Only create the graphs"
echo "3. Do both"
read -p "Enter your choice (1/2/3): " choice

case $choice in
    1)
        echo "Getting data..."
        createDirs
        readIterations
        runAllSudoku
        ;;
    2)
        echo "Drawing graphs..."
        createDirs
        changePerms
        Rscript ./RData/R/Sudoku.R
        ;;
    3)
        echo "Getting data and drawing graphs..."
        createDirs
        readIterations
        runAllSudoku
        Rscript ./RData/R/Sudoku.R
        ;;

    *)
        echo "Invalid choice. Please run the script again and choose 1, 2, or 3."
        exit 1
        ;;
esac

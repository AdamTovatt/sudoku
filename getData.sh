#!/usr/bin/env bash

# Constants
Rdir="./RData/R/"
baseDataDir="./RData/data/raw-data"
graphDir="./RData/data/graphs"
libsDir="./RData/R/r_libs"

# Define solvers and initialize iteration map
declare -A solvers=(
  ["BruteForceAlgorithm"]="$baseDataDir/BruteForceAlgorithm"
  ["MVRAlgorithm"]="$baseDataDir/MVRAlgorithm"
  ["MVRAlgorithm2"]="$baseDataDir/MVRAlgorithm2"
  ["PreprocessAlgorithm"]="$baseDataDir/PreprocessAlgorithm"
)
declare -A iterations

# Create necessary directories
createDirs () {
  for dir in "${solvers[@]}"; do
    mkdir -p "$dir"
  done
  mkdir -p "$graphDir"
  mkdir -p "$libsDir"
  echo "Created necessary directories."
}

# Change R script permissions
changePerms () {
  if chmod +x "${Rdir}Sudoku.R"; then
    echo "Changed permissions."
  else
    echo "Permission change for Sudoku.R failed"
  fi
}

# Prompt for number of iterations per solver
readIterations () {
  echo "Enter how many times you want each algorithm to run:"
  for solver in "${!solvers[@]}"; do
    while true; do
      read -p "$solver: " count
      if [[ "$count" =~ ^[0-9]+$ ]]; then
        iterations["$solver"]=$count
        break
      else
        echo "Please enter a valid number."
      fi
    done
  done
}

# Run the data collection. Just have to run dotnet build
runSudoku() {
  dotnet run --no-build --project ./SudokuCli/SudokuCli.csproj -- --algorithm "$1" --difficulty "$2" --count "$3" --output "$4"
}

runAllSudoku() {
  local difficulties=("Easy" "Medium" "Hard" "Expert")

  for solver in "${!solvers[@]}"; do
    for diff in "${difficulties[@]}"; do
      count="${iterations[$solver]}"
      outdir="${solvers[$solver]}"
      runSudoku "$solver" "$diff" "$count" "$outdir" \
        && echo "$solver $diff Success!" \
        || echo "$solver $diff Failed (exit code: $?)."
    done
  done
}

# Menu
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
    Rscript "${Rdir}Sudoku.R"
    ;;
  3)
    echo "Getting data and drawing graphs..."
    createDirs
    readIterations
    runAllSudoku
    changePerms
    Rscript "${Rdir}Sudoku.R"
    ;;
  *)
    echo "Invalid choice. Please run the script again and choose 1, 2, or 3."
    exit 1
    ;;
esac

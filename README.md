# Sudoku

A project for the EDAA35 course focused on building a Sudoku solver and evaluating different solving algorithms based on performance.

## Requirements

- .NET 8 or later 
- R (Tested on 4.5.0)

## Running Tests

```bash
dotnet test
```

## Running the experiment

The program is executed via an interactive shell script `getData.sh`.  
The script needs to be marked as executable. On linux this can be done with `chomod +x getData.sh`.  
For the script to work the project needs to be built with `dotnet build`.  
Mind that the first run of graph creation may be slow as Rscript downloads and compiles additional packages in the `./RData/R/r_libs` directory.  
As for the recommended number of puzzles that can be solved, the maximum is 50 000 and the data is based on that. On one of our computers that many puzzles took approximately 20 minutes.  
The output of the program resides in two directories, `./RData/data/graphs` and `./RData/data/raw-data`. 

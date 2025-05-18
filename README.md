# Sudoku

A project for the EDAA35 course focused on building a Sudoku solver and evaluating different solving algorithms based on performance and memory usage.

## Requirements

- .NET 8 or later

## Running Tests

```bash
dotnet test
```

## Experimental Branch Notes

The experimental branch doesn't measure RAM usage!

For `getData.sh` to work you need to publish the projects. For windows the command is:
`dotnet publish -c Release -r win-x64 --self-contained false -o`, I think. Self-contained means .NET 8 is still required. 

The experimental branch is much faster so, on a good computer, don't be worried to run 50k for all algorithms, took about 10 minutes on my computer.

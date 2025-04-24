namespace Sudoku.Resources
{
    /// <summary>
    /// Represents an embedded resource with a file path.
    /// </summary>
    public readonly partial struct Resource
    {
        /// <summary>
        /// Gets the path to the resource.
        /// </summary>
        public string Path { get; }

        private Resource(string path)
        {
            Path = path;
        }

        /// <summary>
        /// Creates a <see cref="Resource"/> instance from a known-valid resource path.
        /// 
        /// Warning: This method is unsafe and should only be used when it is certain that the resource path is valid.
        /// Typically used for deserializing previously serialized resource paths.
        /// </summary>
        /// <param name="resourcePath">A known-valid resource path.</param>
        /// <returns>A new <see cref="Resource"/> instance.</returns>
        public static Resource CreateUnverified(string resourcePath)
        {
            return new Resource(resourcePath);
        }

        /// <summary>
        /// Gets the file name component of the resource path.
        /// </summary>
        /// <returns>The file name of the resource.</returns>
        public string GetFileName()
        {
            return System.IO.Path.GetFileName(Path);
        }

        /// <summary>
        /// Returns the string representation of the resource path.
        /// </summary>
        /// <returns>The resource path as a string.</returns>
        public override string ToString() => Path;

        /// <summary>
        /// Implicitly converts a <see cref="Resource"/> to its path string.
        /// </summary>
        /// <param name="resourcePath">The <see cref="Resource"/> to convert.</param>
        public static implicit operator string(Resource resourcePath) => resourcePath.Path;

        /// <summary>
        /// Represents the SudokuPuzzles directory with embedded resource sudoku puzzles.
        /// </summary>
        public static class SudokuPuzzles
        {
            /// <summary>
            /// The file with easy puzzles.
            /// </summary>
            public static readonly Resource Easy = new Resource("SudokuPuzzles/sudoku-9x9-easy.csv");

            /// <summary>
            /// The file with medium puzzles.
            /// </summary>
            public static readonly Resource Medium = new Resource("SudokuPuzzles/sudoku-9x9-medium.csv");

            /// <summary>
            /// The file with hard puzzles.
            /// </summary>
            public static readonly Resource Hard = new Resource("SudokuPuzzles/sudoku-9x9-hard.csv");

            /// <summary>
            /// The file with expert puzzles.
            /// </summary>
            public static readonly Resource Expert = new Resource("SudokuPuzzles/sudoku-9x9-expert.csv");
        }
    }
}

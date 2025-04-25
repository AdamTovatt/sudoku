namespace SudokuCli.Exporting
{
    /// <summary>
    /// Represents a column header in a <see cref="DataTable"/>.
    /// </summary>
    public class DataTableHeaderCell
    {
        /// <summary>
        /// Gets the column name.
        /// </summary>
        public string ColumnName { get; }

        /// <summary>
        /// Gets the data type of the column.
        /// </summary>
        public Type DataType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTableHeaderCell"/> class.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="dataType">The data type of the column.</param>
        public DataTableHeaderCell(string columnName, Type dataType)
        {
            ColumnName = columnName;
            DataType = dataType;
        }

        /// <summary>
        /// Returns a string representation of the header.
        /// </summary>
        public override string ToString()
        {
            return $"{ColumnName} ({DataType.Name})";
        }
    }
}

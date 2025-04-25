using System.Text;

namespace SudokuCli.Exporting
{
    /// <summary>
    /// Represents a single row in a <see cref="DataTable"/>.
    /// </summary>
    public class DataTableRow
    {
        /// <summary>
        /// Gets the values for this row.
        /// </summary>
        public List<object?> Values { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTableRow"/> class.
        /// </summary>
        /// <param name="values">The values for the row.</param>
        public DataTableRow(List<object?> values)
        {
            Values = values;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            int currentIndex = 0;
            int maxIndex = Values.Count - 1;

            foreach (object? value in Values)
            {
                if (value is null)
                    stringBuilder.Append("(null)");
                else
                    stringBuilder.Append(value.ToString());

                if (currentIndex < maxIndex)
                    stringBuilder.Append(", ");

                currentIndex++;
            }

            return stringBuilder.ToString();
        }
    }
}

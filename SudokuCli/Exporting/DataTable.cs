using System.Text;

namespace SudokuCli.Exporting
{
    /// <summary>
    /// Represents a table of data, including column headers and rows.
    /// </summary>
    public class DataTable
    {
        /// <summary>
        /// Gets the headers for the table, including column names and data types.
        /// </summary>
        public List<DataTableHeaderCell> Headers { get; }

        /// <summary>
        /// Gets the rows of data in the table.
        /// </summary>
        public List<DataTableRow> Rows { get; }

        public bool HasColumns => Headers != null && Headers.Count > 0;
        public bool HasRows => Rows != null && Rows.Count > 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTable"/> class.
        /// </summary>
        /// <param name="headers">The column headers.</param>
        public DataTable(List<DataTableHeaderCell> headers)
        {
            Headers = headers;
            Rows = new List<DataTableRow>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTable"/> class.
        /// </summary>
        /// <param name="headers">The column headers.</param>
        /// <param name="rows">The rows in the table.</param>
        public DataTable(List<DataTableHeaderCell> headers, List<DataTableRow> rows)
        {
            Headers = headers;
            Rows = rows;
        }

        /// <summary>
        /// Adds a new row to the table.
        /// </summary>
        /// <param name="row">The row to add.</param>
        public void AddRow(DataTableRow row)
        {
            if (row.Values.Count != Headers.Count)
                throw new ArgumentException("Row column count must match header count.");

            Rows.Add(row);
        }

        /// <summary>
        /// Gets the headers as a string.
        /// </summary>
        /// <param name="delimiter">The value to put between the headers. Defaults to a comma.</param>
        public string GetHeadersAsString(string delimiter = ",")
        {
            List<string> headerValues = new List<string>();

            foreach (DataTableHeaderCell header in Headers)
            {
                headerValues.Add(EscapeCsvValue(header.ColumnName));
            }

            return string.Join(delimiter, headerValues);
        }

        /// <summary>
        /// Exports the data table to CSV format.
        /// </summary>
        /// <param name="includeHeaders">If true, includes headers in the CSV output.</param>
        /// <returns>A CSV-formatted string representing the data table.</returns>
        public string ToCsv(bool includeHeaders = true)
        {
            StringBuilder csvBuilder = new StringBuilder();
            string delimiter = ",";

            // Add headers if needed
            if (includeHeaders)
            {
                List<string> headerValues = new List<string>();

                foreach (DataTableHeaderCell header in Headers)
                {
                    headerValues.Add(EscapeCsvValue(header.ColumnName));
                }

                csvBuilder.Append(string.Join(delimiter, headerValues) + "\n"); // Force `\n`
            }

            // Add rows
            foreach (DataTableRow row in Rows)
            {
                List<string> rowValues = new List<string>();

                foreach (object? value in row.Values)
                {
                    rowValues.Add(EscapeCsvValue(value));
                }

                csvBuilder.Append(string.Join(delimiter, rowValues) + "\n"); // Force `\n`
            }

            return csvBuilder.ToString();
        }

        /// <summary>
        /// Escapes a CSV value to ensure proper formatting.
        /// </summary>
        /// <param name="value">The value to escape.</param>
        /// <returns>A properly formatted CSV value.</returns>
        private static string EscapeCsvValue(object? value)
        {
            if (value == null)
            {
                return "";
            }

            string stringValue = value.ToString() ?? "";

            // Escape values that contain commas, quotes, or newlines
            if (stringValue.Contains(",") || stringValue.Contains("\"") || stringValue.Contains("\n"))
            {
                stringValue = "\"" + stringValue.Replace("\"", "\"\"") + "\"";
            }

            return stringValue;
        }

        /// <summary>
        /// Parses a CSV string into a <see cref="DataTable"/>.
        /// Assumes the first row contains headers.
        /// </summary>
        /// <param name="csv">The CSV string to parse.</param>
        /// <param name="columnTypes">A dictionary of column names and their expected types. If null, all columns are treated as strings.</param>
        /// <returns>A populated <see cref="DataTable"/>.</returns>
        public static DataTable FromCsv(string csv, Dictionary<string, Type>? columnTypes = null)
        {
            if (string.IsNullOrWhiteSpace(csv))
                throw new ArgumentException("CSV input cannot be empty.", nameof(csv));

            using (StringReader reader = new StringReader(csv))
            {
                string? headerLine = ReadNextCsvLine(reader);
                if (string.IsNullOrEmpty(headerLine))
                    throw new ArgumentException("CSV input does not contain valid headers.", nameof(csv));

                // Read headers
                string[] headerNames = ParseCsvLine(headerLine);
                List<DataTableHeaderCell> headers = new List<DataTableHeaderCell>();

                foreach (string columnName in headerNames)
                {
                    if (string.IsNullOrEmpty(columnName)) continue; // Ignore empty column names

                    Type columnType = typeof(string); // Default type is string
                    if (columnTypes != null && columnTypes.TryGetValue(columnName, out Type? providedType))
                    {
                        columnType = providedType;
                    }

                    headers.Add(new DataTableHeaderCell(columnName, columnType));
                }

                DataTable dataTable = new DataTable(headers);

                // Read data rows
                string? line;
                while ((line = ReadNextCsvLine(reader)) != null)
                {
                    string[] rowValues = ParseCsvLine(line);
                    List<object?> parsedValues = new List<object?>();

                    for (int j = 0; j < headers.Count; j++)
                    {
                        string cellValue = j < rowValues.Length ? rowValues[j] : "";
                        parsedValues.Add(ConvertCsvValue(cellValue, headers[j].DataType));
                    }

                    dataTable.AddRow(new DataTableRow(parsedValues));
                }

                return dataTable;
            }
        }

        /// <summary>
        /// Reads the next valid CSV line from the reader, ensuring multi-line quoted fields are handled correctly.
        /// </summary>
        private static string? ReadNextCsvLine(StringReader reader)
        {
            StringBuilder lineBuilder = new StringBuilder();
            string? line;

            while ((line = reader.ReadLine()) != null)
            {
                if (lineBuilder.Length > 0)
                    lineBuilder.Append("\n"); // Preserve newlines inside quotes

                lineBuilder.Append(line);
                int quoteCount = lineBuilder.ToString().Count(c => c == '"');

                if (quoteCount % 2 == 0) // Even number of quotes = valid CSV line
                    return lineBuilder.ToString();
            }

            return lineBuilder.Length > 0 ? lineBuilder.ToString() : null;
        }

        /// <summary>
        /// Parses a CSV line into an array of values, handling quoted values properly.
        /// </summary>
        /// <param name="csvLine">A single line from a CSV file.</param>
        /// <returns>An array of values.</returns>
        private static string[] ParseCsvLine(string csvLine)
        {
            List<string> values = new List<string>();
            StringBuilder current = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < csvLine.Length; i++)
            {
                char c = csvLine[i];

                if (c == '\"') // Handle quoted values
                {
                    if (inQuotes && i + 1 < csvLine.Length && csvLine[i + 1] == '\"')
                    {
                        // Escaped double quote ("" → ")
                        current.Append('\"');
                        i++; // Skip next quote
                    }
                    else
                    {
                        inQuotes = !inQuotes; // Toggle quote mode
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    // End of field
                    values.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }

            // Add last value
            values.Add(current.ToString());
            return values.ToArray();
        }

        /// <summary>
        /// Converts a CSV string value into the specified type.
        /// </summary>
        /// <param name="value">The CSV value as a string.</param>
        /// <param name="targetType">The target type.</param>
        /// <returns>The converted value.</returns>
        private static object? ConvertCsvValue(string value, Type targetType)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null; // Treat empty values as null

            try
            {
                if (targetType == typeof(int))
                    return int.Parse(value);
                if (targetType == typeof(long))
                    return long.Parse(value);
                if (targetType == typeof(double))
                    return double.Parse(value);
                if (targetType == typeof(bool))
                    return bool.Parse(value);
                if (targetType == typeof(DateTime))
                    return DateTime.Parse(value);
                if (targetType == typeof(Guid))
                    return Guid.Parse(value);

                return value; // Default to string if type is unknown
            }
            catch
            {
                return value; // If conversion fails, return the original string
            }
        }

        /// <summary>
        /// Checks if the headers of this DataTable match the headers of another DataTable.
        /// </summary>
        /// <param name="other">The DataTable to compare with.</param>
        /// <returns>True if both tables have the same headers, otherwise false.</returns>
        public bool HasSameHeaders(DataTable other)
        {
            if (other == null) return false;
            if (Headers.Count != other.Headers.Count) return false;

            return Headers.Select(h => h.ColumnName).SequenceEqual(other.Headers.Select(h => h.ColumnName));
        }

        /// <summary>
        /// Checks if this DataTable has the same rows as another DataTable.
        /// </summary>
        /// <param name="other">The DataTable to compare with.</param>
        /// <param name="excludedColumns">Optional parameter for columns to exclude in the comparisson. The columnsthat are send in this parameter won't be accounted for when a check for row equality is performed.</param>
        /// <returns>True if both tables contain the same rows, otherwise false.</returns>
        public bool HasSameRows(DataTable other, params int[] excludedColumns)
        {
            if (other == null) return false;
            if (!HasSameHeaders(other)) return false;
            if (Rows.Count != other.Rows.Count) return false;

            if (excludedColumns.Length == 0)
            {
                HashSet<string> thisRowHashes = new HashSet<string>(Rows.Select(r => ComputeRowHash(r)));
                HashSet<string> otherRowHashes = new HashSet<string>(other.Rows.Select(r => ComputeRowHash(r)));

                return thisRowHashes.SetEquals(otherRowHashes);
            }
            else
            {
                HashSet<string> thisRowHashes = new HashSet<string>(Rows.Select(r => ComputeRowHash(r, excludedColumns)));
                HashSet<string> otherRowHashes = new HashSet<string>(other.Rows.Select(r => ComputeRowHash(r, excludedColumns)));

                return thisRowHashes.SetEquals(otherRowHashes);
            }
        }

        /// <summary>
        /// Gets the rows that differ between this DataTable and another DataTable.
        /// </summary>
        /// <param name="other">The DataTable to compare with.</param>
        /// <param name="excludedColumns">Optional parameter for columns to exclude in the comparisson. The columnsthat are send in this parameter won't be accounted for when a check for row equality is performed.</param>
        /// <returns>A new DataTable containing the differing rows.</returns>
        public DataTable GetDifferingRows(DataTable other, params int[] excludedColumns)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (!HasSameHeaders(other)) throw new ArgumentException("Data tables must have the same structure (same headers).");

            DataTable differingRows = new DataTable(new List<DataTableHeaderCell>(Headers));

            HashSet<string> otherRowHashes = new HashSet<string>(other.Rows.Select(r => ComputeRowHash(r, excludedColumns)));

            foreach (DataTableRow row in Rows)
            {
                if (!otherRowHashes.Contains(ComputeRowHash(row, excludedColumns)))
                {
                    differingRows.AddRow(new DataTableRow(new List<object?>(row.Values)));
                }
            }

            return differingRows;
        }

        /// <summary>
        /// Gets the columns that differ between this <see cref="DataTable"/> and another <see cref="DataTable"/>.
        /// Returns a new <see cref="DataTable"/> containing only the differing columns and their values for all rows.
        /// Both tables must have the same structure and row count.
        /// </summary>
        /// <param name="other">The <see cref="DataTable"/> to compare with.</param>
        /// <returns>A new <see cref="DataTable"/> with only the differing columns.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the other table is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the tables do not have the same headers or row count.</exception>
        public DataTable GetDifferingColumns(DataTable other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (!HasSameHeaders(other)) throw new ArgumentException("Data tables must have the same structure (same headers).");
            if (Rows.Count != other.Rows.Count) throw new ArgumentException("Data tables must have the same number of rows.");

            List<int> differingColumnIndexes = new List<int>();

            for (int colIndex = 0; colIndex < Headers.Count; colIndex++)
            {
                for (int rowIndex = 0; rowIndex < Rows.Count; rowIndex++)
                {
                    object? value1 = Rows[rowIndex].Values[colIndex];
                    object? value2 = other.Rows[rowIndex].Values[colIndex];

                    if (!Equals(value1, value2))
                    {
                        differingColumnIndexes.Add(colIndex);
                        break;
                    }
                }
            }

            List<DataTableHeaderCell> differingHeaders = new List<DataTableHeaderCell>();
            foreach (int index in differingColumnIndexes)
            {
                differingHeaders.Add(Headers[index]);
            }

            DataTable result = new DataTable(differingHeaders);

            foreach (DataTableRow row in Rows)
            {
                List<object?> values = new List<object?>();
                foreach (int index in differingColumnIndexes)
                {
                    values.Add(row.Values[index]);
                }

                result.AddRow(new DataTableRow(values));
            }

            return result;
        }

        /// <summary>
        /// Gets the index of the first column with the specified name.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The zero-based index of the column if found; otherwise, null.</returns>
        public int? GetFirstIndexOfColumnByName(string name)
        {
            for (int i = 0; i < Headers.Count; i++)
            {
                if (Headers[i].ColumnName == name) return i;
            }

            return null;
        }

        /// <summary>
        /// Computes the differences between this (original) <see cref="DataTable"/> and another (modified) <see cref="DataTable"/> at the cell level.
        /// Returns a <see cref="DataTableDelta"/> containing all cells where values differ.
        /// Both tables must have the same structure and number of rows.
        /// </summary>
        /// <param name="other">The <see cref="DataTable"/> to compare with. This should be the new version while the object that this method is called on should be the original.</param>
        /// <returns>A <see cref="DataTableDelta"/> representing the cell-level differences.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the other table is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the tables do not have the same headers or row count.</exception>
        public DataTableDelta GetDelta(DataTable other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (!HasSameHeaders(other)) throw new ArgumentException("Data tables must have the same structure (same headers).");
            if (Rows.Count != other.Rows.Count) throw new ArgumentException("Data tables must have the same number of rows.");

            List<CellDelta> differences = new List<CellDelta>();

            for (int rowIndex = 0; rowIndex < Rows.Count; rowIndex++)
            {
                for (int colIndex = 0; colIndex < Headers.Count; colIndex++)
                {
                    object? originalValue = Rows[rowIndex].Values[colIndex];
                    object? newValue = other.Rows[rowIndex].Values[colIndex];

                    if (!Equals(originalValue, newValue))
                    {
                        string columnName = Headers[colIndex].ColumnName;
                        differences.Add(new CellDelta(rowIndex, colIndex, columnName, originalValue, newValue));
                    }
                }
            }

            return new DataTableDelta(differences);
        }

        /// <summary>
        /// Computes a hash for a row based on its values.
        /// </summary>
        /// <param name="row">The row to hash.</param>
        /// <param name="excludedColumns">Optional parameter for columns to exclude in the comparisson. The columnsthat are send in this parameter won't be accounted for when a check for row equality is performed.</param>
        /// <returns>A string representing the row hash.</returns>
        private string ComputeRowHash(DataTableRow row, params int[] excludedColumns)
        {
            if (excludedColumns.Length > 0)
            {
                HashSet<int> excludedIndexes = excludedColumns.ToHashSet();

                return string.Join("|",
                    row.Values
                        .Select((v, i) => new { v, i })
                        .Where(x => !excludedIndexes.Contains(x.i))
                        .Select(x => x.v?.ToString() ?? "NULL"));
            }
            else
            {
                return string.Join("|", row.Values.Select(v => v?.ToString() ?? "NULL"));
            }
        }
    }
}

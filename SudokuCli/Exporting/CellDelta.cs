using System.Text.Json.Serialization;

namespace SudokuCli.Exporting
{
    public class CellDelta
    {
        [JsonPropertyName("rowIndex")]
        public int RowIndex { get; }

        [JsonPropertyName("columnIndex")]
        public int ColumnIndex { get; }

        [JsonPropertyName("columnName")]
        public string ColumnName { get; }

        [JsonPropertyName("originalValue")]
        public object? OriginalValue { get; }

        [JsonPropertyName("newValue")]
        public object? NewValue { get; }

        public CellDelta(int rowIndex, int columnIndex, string columnName, object? originalValue, object? newValue)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            ColumnName = columnName;
            OriginalValue = originalValue;
            NewValue = newValue;
        }

        public override string ToString()
        {
            return $"{ColumnName} changed from: \"{OriginalValue}\" to \"{NewValue}\"";
        }
    }
}

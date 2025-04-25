using System.Text.Json.Serialization;
using System.Text.Json;

namespace SudokuCli.Exporting
{
    public class DataTableDelta
    {
        [JsonPropertyName("differences")]
        public List<CellDelta> Differences { get; }

        public DataTableDelta(List<CellDelta> differences)
        {
            Differences = differences;
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public static DataTableDelta FromJson(string json)
        {
            DataTableDelta? result = JsonSerializer.Deserialize<DataTableDelta>(json);

            if (result == null || result.Differences == null)
                throw new ArgumentException($"Failed to deserialize {nameof(DataTableDelta)} from json {json}");

            return result;
        }
    }
}

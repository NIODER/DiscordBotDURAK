using System.Text.Json;
using System.Text.Json.Serialization;

namespace DatabaseModel
{
    public class Symbol
    {
        public ulong SymbolId { get; set; }
        public string? Content { get; set; }

        [JsonIgnore]
        public List<SymbolsList> SymbolsLists { get; set; } = new();
        [JsonIgnore]
        public List<SymbolsListsToSymbols> SymbolsListsToSymbols { get; set; } = new();

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}

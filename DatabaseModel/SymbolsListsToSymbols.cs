using System.Text.Json;
using System.Text.Json.Serialization;

namespace DatabaseModel
{
    public class SymbolsListsToSymbols
    {
        public ulong SymbolId { get; set; }
        [JsonIgnore]
        public Symbol? SymbolNavigation { get; set; }

        public ulong ListId { get; set; }
        [JsonIgnore]
        public SymbolsList? SymbolsListNavigation { get; set; }

        public bool IsExcluded { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}

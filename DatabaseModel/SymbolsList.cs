using System.Text.Json;
using System.Text.Json.Serialization;

namespace DatabaseModel
{
    public class SymbolsList
    {
        public ulong ListId { get; set; }
        public string? Title { get; set; }
        
        [JsonIgnore]
        public List<Symbol> Symbols { get; set; } = new();
        [JsonIgnore]
        public List<Channel> Channels { get; set; } = new();
        [JsonIgnore]
        public List<Guild> Guilds { get; set; } = new();
        [JsonIgnore]
        public List<SymbolsListsToChannels> SymbolsListsToChannels { get; set; } = new();
        [JsonIgnore]
        public List<SymbolsListsToSymbols> SymbolsListsToSymbols { get; set; } = new();

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}

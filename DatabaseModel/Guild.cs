using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DatabaseModel
{
    public class Guild
    {
        [NotMapped, JsonIgnore]
        public const short DEFAULT_SPY_MODE = 0;
        [NotMapped, JsonIgnore]
        public const ulong DEFAULT_BASE_ROLE = 0;
        [NotMapped, JsonIgnore]
        public const ulong DEFAULT_IMMUNITY_ROLE = 0;

        public ulong GuildId { get; set; }
        public short SpyMode { get; set; }
        public ulong BaseRole { get; set; }
        public ulong? ImmunityRole { get; set; }

        [JsonIgnore]
        public List<GuildUser> GuildUsers { get; set; } = new();
        [JsonIgnore]
        public List<Channel> Channels { get; set; } = new();
        [JsonIgnore]
        public List<SymbolsList> SymbolsLists { get; set; } = new();

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}

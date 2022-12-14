using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DatabaseModel
{
    public class Channel
    {
        [NotMapped, JsonIgnore]
        public const short DEFAULT_MODERATION = 0;
        [NotMapped, JsonIgnore]
        public const ulong DEFAULT_RESEND_CHANNEL = 0;

        public ulong ChannelId { get; set; }
        public ulong GuildId { get; set; }
        public short Moderation { get; set; }
        public string? Warning { get; set; }

        [JsonIgnore]
        public Guild? GuildNavigation { get; set; }
        [JsonIgnore]
        public List<SymbolsList> SymbolsLists { get; set; } = new();
        [JsonIgnore]
        public List<SymbolsListsToChannels> SymbolsListsToChannels { get; set; } = new();

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DatabaseModel
{
    public class SymbolsListsToChannels
    {
        [NotMapped, JsonIgnore]
        public const short DEFAULT_MODERATION = 0;

        public ulong ChannelId { get; set; }
        [JsonIgnore]
        public Channel? ChannelNavigation { get; set; }

        public ulong ListId { get; set; }
        [JsonIgnore]
        public SymbolsList? SymbolsListNavigation { get; set; }

        public short Moderation { get; set; }
        public ulong? ResendChannelId { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}

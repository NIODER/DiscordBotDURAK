using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DatabaseModel
{
    public class GuildUser
    {
        [NotMapped, JsonIgnore]
        public const short DEFAULT_ROLE = 1;

        public ulong UserId { get; set; }
        public ulong GuildId { get; set; }
        public DateTime? LastActiveAt { get; set; }
        public short Role { get; set; }
        public bool HasImmunity { get; set; }
        public string? Invited { get; set; }
        public string? Introduced { get; set; }

        [JsonIgnore]
        public Guild? GuildNavigation { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}

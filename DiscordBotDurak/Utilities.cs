using DatabaseModel;
using DiscordBotDurak.Enum.ModerationModes;
using DiscordBotDurak.Mentions;
using System.Text.Json;
using System;
using System.Collections.Generic;

namespace DiscordBotDurak
{
    public static class Utilities
    {
        public static (ulong innerId, MentionType type) FromMention(string mention)
        {
            try
            {
                return mention.Trim()[1] switch
                {
                    '#' => (ulong.Parse(mention[2..^1]), MentionType.Channel),
                    '@' => mention[2] == '&'
                        ? (ulong.Parse(mention[3..^1]), MentionType.Role)
                        : (ulong.Parse(mention[2..^1]), MentionType.User),
                    'e' => (0, MentionType.Everyone),
                    'h' => (0, MentionType.Here),
                    _ => (0, MentionType.Error)
                };
            }
            catch (Exception)
            {
                return (0, MentionType.Error);
            }
        }

        public static bool CodeContains(short code, ModerationMode moderationMode)
        {
            return (code & (short)moderationMode) == (short)moderationMode;
        }

        public static List<ulong> GetIds(string ids)
        {
            var res = new List<ulong>();
            foreach (var id in ids.Split(','))
            {
                try
                {
                    res.Add(ulong.Parse(id));
                }
                catch (Exception)
                {
                    continue;
                }
            }
            return res;
        }
    }
}

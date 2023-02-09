namespace DiscordBotDurak
{
    public interface IVerifiable
    {
        bool Verify(ulong userId, ulong guildId);
    }
}

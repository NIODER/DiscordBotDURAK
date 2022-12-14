namespace DiscordBotDurak.Enum.ModerationModes
{
    public enum ModerationMode
    {
        NonModerated = 1,
        OnlyWarnings = 1 << 1,
        OnlyResend = 1 << 2,
        OnlyDelete = 1 << 3
    }
}
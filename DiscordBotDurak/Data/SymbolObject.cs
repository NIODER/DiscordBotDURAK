namespace DiscordBotDurak.Data
{
    public class SymbolObject
    {
        public ulong SymbolId { get; set; }
        public string Content { get; set; }
        public bool IsExcluded { get; set; }
        public ulong ListId { get; set; }

        public SymbolObject(ulong symbolId, string content, bool isExcluded, ulong listId)
        {
            SymbolId = symbolId;
            Content = content;
            IsExcluded = isExcluded;
            ListId = listId;
        }
    }
}

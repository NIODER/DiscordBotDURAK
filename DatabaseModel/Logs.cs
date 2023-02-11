namespace DatabaseModel
{
    public class Logs
    {
        public long LogId { get; set; }
        public short LogSeverity { get; set; }
        public string Source { get; set; } = "unknown";
        public string Message { get; set; } = "unknown";
        public string? Exception { get; set; }
        public DateTime? Created { get; set; }
    }
}

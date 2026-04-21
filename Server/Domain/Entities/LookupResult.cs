namespace Server.Domain.Entities
{
    public class LookupResult
    {
        public int? Value { get; set; }
        public bool Found { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}

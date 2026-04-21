namespace Client.Domain.Entities
{
    public class ServerResponse
    {
        public List<string> Messages { get; set; } = new();
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}

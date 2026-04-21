namespace Server.Domain.Interfaces
{
    public interface IDataRepository
    {
        int? Lookup(string input);
    }
}

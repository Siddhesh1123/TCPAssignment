using Server.Domain.Interfaces;

namespace Server.Infrastructure.Data
{
    public class DataRepository : IDataRepository
    {
        private static readonly Dictionary<string, Dictionary<string, int>> Collection = new()
        {
            { "SetA", new Dictionary<string, int> { { "One", 1 }, { "Two", 2 } } },
            { "SetB", new Dictionary<string, int> { { "Three", 3 }, { "Four", 4 } } },
            { "SetC", new Dictionary<string, int> { { "Five", 5 }, { "Six", 6 } } },
            { "SetD", new Dictionary<string, int> { { "Seven", 7 }, { "Eight", 8 } } },
            { "SetE", new Dictionary<string, int> { { "Nine", 9 }, { "Ten", 10 } } }
        };

        public int? Lookup(string input)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input))
                    return null;

                var parts = input.Split('-');
                if (parts.Length != 2) 
                    return null;

                string setKey = parts[0].Trim();
                string valueKey = parts[1].Trim();

                if (string.IsNullOrWhiteSpace(setKey) || string.IsNullOrWhiteSpace(valueKey))
                    return null;

                if (Collection.TryGetValue(setKey, out var subset))
                {
                    if (subset.TryGetValue(valueKey, out int value))
                        return value;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DataRepository.Lookup: {ex.Message}");
                return null;
            }
        }
    }
}

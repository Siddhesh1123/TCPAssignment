namespace Server
{
    public static class DataStore
    {
        // Predefined collection of sets and their values
        private static readonly Dictionary<string, Dictionary<string, int>> Collection = new()
        {
            { "SetA", new Dictionary<string, int> { { "One", 1 }, { "Two", 2 } } },
            { "SetB", new Dictionary<string, int> { { "Three", 3 }, { "Four", 4 } } },
            { "SetC", new Dictionary<string, int> { { "Five", 5 }, { "Six", 6 } } },
            { "SetD", new Dictionary<string, int> { { "Seven", 7 }, { "Eight", 8 } } },
            { "SetE", new Dictionary<string, int> { { "Nine", 9 }, { "Ten", 10 } } }
        };

        public static int? Lookup(string input)
        {
            // Split "SetA-Two" into ["SetA", "Two"]
            var parts = input.Split('-');
            if (parts.Length != 2) return null;

            string setKey = parts[0].Trim();
            string valueKey = parts[1].Trim();

            if (Collection.TryGetValue(setKey, out var subset))
            {
                if (subset.TryGetValue(valueKey, out int value))
                    return value;
            }

            return null; // Not found → send EMPTY
        }
    }
}
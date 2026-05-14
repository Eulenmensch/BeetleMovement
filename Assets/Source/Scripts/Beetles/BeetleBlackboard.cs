using System.Collections.Generic;

namespace Source.Beetles
{
    // Key constants for BeetleBlackboard entries.
    public static class BK
    {
        public const string Hunger            = "Hunger";            // float 0–1
        public const string Health            = "Health";            // float 0–1 (fraction of max)
        public const string NearestFood       = "NearestFood";       // FoodSource
        public const string NearestThreat     = "NearestThreat";     // Transform (predator)
        public const string NearestPrey       = "NearestPrey";       // Transform (cattle beetle)
        public const string NearestPreyHealth = "NearestPreyHealth"; // BeetleHealth
        public const string ThreatDistance    = "ThreatDistance";    // float, world units
        public const string PheromoneStrength = "PheromoneStrength"; // float 0–1
        public const string PheromoneDir      = "PheromoneDir";      // Vector3
        public const string AtFoodSource      = "AtFoodSource";      // bool
        public const string AtPrey            = "AtPrey";            // bool
    }

    // Lightweight key-value store shared between BeetleSensor and BeetleBrain actions.
    public class BeetleBlackboard
    {
        private readonly Dictionary<string, object> _data = new();

        public void Set<T>(string key, T value) => _data[key] = value;

        public T Get<T>(string key, T defaultValue = default)
        {
            if (_data.TryGetValue(key, out object v) && v is T typed) return typed;
            return defaultValue;
        }

        public bool Has(string key) => _data.ContainsKey(key);
    }
}

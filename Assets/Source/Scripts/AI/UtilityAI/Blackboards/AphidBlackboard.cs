using System.Collections.Generic;

namespace Source.AI.UtilityAI
{
	public class AphidBlackboard : IBlackboard
	{
		private Dictionary<string, object> data = new();

		public void Set<T>(string key, T value) => data[key] = value;

		public T Get<T>(string key) => data.TryGetValue(key, out var value) ? (T)value : default;

		public bool Has(string key) => data.ContainsKey(key);
	}
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Source.AI.UtilityAI
{
	[Serializable]
	public class Percept
	{
		public string Type;
		public Vector3 Position;
		public Dictionary<string, object> Data;

		public Percept(string type, Vector3 position, Dictionary<string, object> data = null)
		{
			Type = type;
			Position = position;
			Data = data ?? new Dictionary<string, object>();
		}

		public T Get<T>(string key, T defaultValue = default)
		{
			if (Data.TryGetValue(key, out var value) && value is T typed)
			{
				return typed;
			}

			return defaultValue;
		}

		public bool Has(string key) => Data.ContainsKey(key);

		public override string ToString()
		{
			return $"{Type} at {Position} ({Data.Count} fields)";
		}
	}
}
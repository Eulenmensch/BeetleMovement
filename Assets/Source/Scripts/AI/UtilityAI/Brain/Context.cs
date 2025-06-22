using System.Collections.Generic;
using Source.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace Source.AI.UtilityAI
{
	public class Context
	{
		public Brain brain;
		public NavMeshAgent agent;
		public Transform target;
		public Sensor sensor;

		private readonly Dictionary<string, object> data = new();

		public Context(Brain brain)
		{
			if (brain == null) return;

			this.brain = brain;
			this.agent = brain.gameObject.GetOrAdd<NavMeshAgent>();
			this.sensor = brain.gameObject.GetOrAdd<Sensor>();
		}

		public T GetData<T>(string key) => data.TryGetValue(key, out var value) ? (T)value : default;
		public void SetData(string key, object value) => data[key] = value;
	}
}
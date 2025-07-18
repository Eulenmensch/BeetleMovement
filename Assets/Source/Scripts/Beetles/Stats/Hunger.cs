using System;
using Source.AI.UtilityAI;
using UnityEngine;

namespace Source.Beetles.Stats {
	[RequireComponent(typeof(IBlackboard))]
	public class Hunger : MonoBehaviour {
		public float maxHunger = 100;
		public float minHunger = 0;
		public float current;
		public float timeToZeroInSeconds = 60;
		private float NormalizedHunger => current / maxHunger;
		private IBlackboard agentBlackboard;
		void Start() {
			current = minHunger;
			agentBlackboard = GetComponent<IBlackboard>();
		}

		private void Update()
		{
			AddHunger(Time.deltaTime * maxHunger / timeToZeroInSeconds);
		}

		public void ReduceHunger(float hunger)
		{
			current -= hunger;
			if (current < 0)
			{
				current = 0;
			}
			agentBlackboard.Set(InsectKeys.HungerAmount, NormalizedHunger);
		}

		private void AddHunger(float hunger) {
			current += hunger;
			if (current >= maxHunger) {
				Die();
			}
			agentBlackboard.Set(InsectKeys.HungerAmount, NormalizedHunger);
		}

		void Die() {
			Destroy(gameObject);
		}
	}
}
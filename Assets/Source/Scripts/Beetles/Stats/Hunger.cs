using System;
using UnityEngine;

namespace Source.Beetles.Stats {
	public class Hunger : MonoBehaviour {
		public float maxHunger = 100;
		public float minHunger = 0;
		public float current;
		public float timeToZeroInSeconds = 60;
		public float NormalizedHunger => current / maxHunger;

		void Start() {
			current = minHunger;
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
		}

		public void AddHunger(float hunger) {
			current += hunger;
			if (current >= maxHunger) {
				Die();
			}
		}

		void Die() {
			Destroy(gameObject);
		}
	}
}
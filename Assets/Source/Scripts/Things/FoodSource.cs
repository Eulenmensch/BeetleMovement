using System;
using UnityEngine;

namespace Source.Things
{
	public class FoodSource : MonoBehaviour
	{
		[field:SerializeField] public float Capacity { set; get; }
		[SerializeField] private float refillRatePerSecond;

		public float CurrentAmount { private set; get; }

		private Vector3 defaultScale;

		private void Start()
		{
			defaultScale = transform.localScale;
			CurrentAmount = Capacity;
		}

		public void Drain(float amount)
		{
			CurrentAmount -= amount;
			if (CurrentAmount <= 0)
			{
				CurrentAmount = 0;
			}
		}

		private void Refill(float amount)
		{
			if (CurrentAmount >= Capacity) return;
			CurrentAmount += amount;
			if (CurrentAmount >= Capacity)
			{
				CurrentAmount = Capacity;
			}
		}

		private void ResizeWithFillRatio()
		{
			var ratio = CurrentAmount / Capacity;
			transform.localScale = defaultScale * ratio;
		}

		private void Update()
		{
			Refill(refillRatePerSecond * Time.deltaTime);
			ResizeWithFillRatio();
		}
	}
}
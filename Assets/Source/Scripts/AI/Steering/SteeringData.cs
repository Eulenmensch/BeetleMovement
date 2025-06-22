using UnityEngine;

namespace Source.AI.Steering
{
	public class SteeringData
	{
		public Vector3 Linear { get; set; }
		public float Angular { get; set; }

		public SteeringData()
		{
			Linear = Vector3.zero;
			Angular = 0f;
		}
	}
}
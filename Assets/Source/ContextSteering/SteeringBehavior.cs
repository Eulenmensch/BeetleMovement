using UnityEngine;

namespace Source.ContextSteering
{
	public abstract class SteeringBehavior : MonoBehaviour
	{
		public abstract (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, AIData aiData);
	}
}
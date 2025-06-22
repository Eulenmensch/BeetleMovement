using UnityEngine;

namespace Source.AI.ContextSteering
{
	public abstract class Detector : MonoBehaviour
	{
		public abstract void Detect(AIData aiData);
	}
}
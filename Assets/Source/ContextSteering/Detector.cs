using UnityEngine;

namespace Source.ContextSteering
{
	public abstract class Detector : MonoBehaviour
	{
		public abstract void Detect(AIData aiData);
	}
}
using UnityEngine;

namespace Source.AI.Steering
{
	public interface IDecideDirection
	{
		Vector3 GetDirection(float[] contextMap);
	}
}
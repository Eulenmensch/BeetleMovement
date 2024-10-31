using UnityEngine;

namespace Source.Steering
{
	public interface IDecideDirection
	{
		Vector3 GetDirection(float[] contextMap);
	}
}
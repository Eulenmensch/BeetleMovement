using UnityEngine;

namespace Source.Steering
{
	public class ContextSteering : IDecideDirection
	{
		public Vector3 GetDirection(float[] contextMap)
        {
            Vector3 direction = Vector3.zero;

            for (var i = 0; i < contextMap.Length; i++)
            {
                float angle = i * (360f / contextMap.Length);
                var dir = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
                direction += dir * contextMap[i];
            }

            return direction.normalized;
        }
	}
}
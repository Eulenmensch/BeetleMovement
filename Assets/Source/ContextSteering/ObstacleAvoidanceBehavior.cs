using UnityEngine;

namespace Source.ContextSteering
{
	public class ObstacleAvoidanceBehavior : SteeringBehavior
	{
		[SerializeField] private float radius, agentColliderSize;
		[SerializeField] private bool showGizmos;
		
		float[] contextMap = null;

		public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, AIData aiData)
		{
			foreach (var obstacle in aiData.obstacles)
			{
				var directionToObstacle = obstacle.ClosestPoint(transform.position) - (Vector2)transform.position;
				var distanceToObstacle = directionToObstacle.magnitude;
				
				var weight = distanceToObstacle <= agentColliderSize ? 1 : (radius - distanceToObstacle) / radius;
				
				var directionToObstacleNormalized = directionToObstacle.normalized;

				foreach (var direction in Directions.eight)
				{
					var directionalInfluence = Vector2.Dot(directionToObstacleNormalized, direction);
					var weightedInfluence = directionalInfluence * weight;

					var index = Directions.eight.IndexOf(direction);
					
					if (weightedInfluence > danger[index])
					{
						danger[index] = weightedInfluence;
					}
				}
			}

			contextMap = danger;
			return (danger, interest);
		}
		
		private void OnDrawGizmos()
		{
			if (showGizmos == false)
				return;

			if (Application.isPlaying && contextMap != null)
			{
				if (contextMap != null)
				{
					Gizmos.color = Color.red;
					for (int i = 0; i < contextMap.Length; i++)
					{
						Gizmos.DrawRay(
							transform.position,
							Directions.eight[i] * contextMap[i]*2
						);
					}
				}
			}

		}
	}
}
using System.Linq;
using UnityEngine;

namespace Source.AI.ContextSteering
{
	public class SeekBehavior : SteeringBehavior
	{
		[SerializeField] private float targetReachedThreshold;
		[SerializeField] private bool showGizmos;

		private bool reachedLastSeenPos = true;
		private Vector2 lastSeenPosition;
		
		public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, AIData aiData)
		{
			if (reachedLastSeenPos)
			{
				if (aiData.targets == null || aiData.targets.Count <= 0)
				{
					aiData.currentTarget = null;
					return (danger, interest);
				}
				reachedLastSeenPos = false;
				aiData.currentTarget = aiData.targets.OrderBy(target => Vector2.Distance(target.position, transform.position)).FirstOrDefault();
			}

			if (aiData.currentTarget && aiData.targets != null && aiData.targets.Contains(aiData.currentTarget))
			{
				lastSeenPosition = aiData.currentTarget.position;
			}

			if (Vector2.Distance(transform.position, lastSeenPosition) < targetReachedThreshold)
			{
				reachedLastSeenPos = true;
				aiData.currentTarget = null;
				return (danger, interest);
			}

			Vector2 directionToTarget = lastSeenPosition - (Vector2)transform.position;
			for (int i = 0; i < interest.Length; i++)
			{
				var directionalInfluence = Vector2.Dot(directionToTarget.normalized, Directions.eight[i]);
				if (directionalInfluence > 0)
				{
					if (directionalInfluence > interest[i])
					{
						interest[i] = directionalInfluence;
					}
				}
			}

			return (danger, interest);
		}
	}
}
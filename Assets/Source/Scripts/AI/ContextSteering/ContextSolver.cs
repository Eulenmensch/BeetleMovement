using System.Collections.Generic;
using UnityEngine;

namespace Source.AI.ContextSteering
{
	public class ContextSolver : MonoBehaviour
	{
		[SerializeField] private bool showGizmos;
		[SerializeField] private float debugRayLength = 1;
		private float[] interestGizmos;
		private Vector2 moveDirection;

		private void Start()
		{
			interestGizmos = new float[8];
		}

		public Vector2 GetMovementDirection(List<SteeringBehavior> behaviors, AIData aiData)
		{
			float[] danger = new float[8];
			float[] interest = new float[8];

			foreach (var behavior in behaviors)
			{
				(danger, interest) = behavior.GetSteering(danger, interest, aiData);
			}

			for (int i = 0; i < interest.Length; i++)
			{
				interest[i] = Mathf.Clamp01(interest[i] - danger[i]);
			}

			interestGizmos = interest;

			Vector2 outputDirection = Vector2.zero;
			for (int i = 0; i < interest.Length; i++)
			{
				outputDirection += Directions.eight[i] * interest[i];
			}

			moveDirection = outputDirection.normalized;
			return moveDirection;
		}
		
		private void OnDrawGizmos()
		{
			if (showGizmos == false)
				return;
			
			if (Application.isPlaying && showGizmos)
			{
				Gizmos.color = Color.yellow;
				Gizmos.DrawRay(transform.position, moveDirection * debugRayLength);
			}

			if (Application.isPlaying && interestGizmos != null)
			{
				if (interestGizmos != null)
				{
					Gizmos.color = Color.red;
					for (int i = 0; i < interestGizmos.Length; i++)
					{
						Gizmos.DrawRay(
							transform.position,
							Directions.eight[i] * interestGizmos[i]*debugRayLength
						);
					}
				}
			}
		}
	}
}
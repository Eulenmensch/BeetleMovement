using System;
using Source.Utils;
using UnityEngine;

namespace Source.AI.UtilityAI
{
	[CreateAssetMenu(menuName = "UtilityAI/Considerations/InRange")]
	public class InRangeConsideration : Consideration
	{
		public float maxDistance = 10f;
		public float maxAngle = 360f;
		public string targetTag = "Target";
		[Tooltip("X axis = input value, y axis = utility")]
		public AnimationCurve curve;
		
		public override float Evaluate(Brain brain, IBlackboard blackboard)
		{
			if (!brain.sensor.targetTags.Contains(targetTag))
			{
				brain.sensor.targetTags.Add(targetTag);
			}

			var targetTransform = brain.sensor.GetClosestTarget(targetTag);
			if (!targetTransform) return 0f;

			var agentTransform = brain.agent.transform;

			var isInRange = agentTransform.InRangeOf(targetTransform, maxDistance, maxAngle);
			if (!isInRange) return 0f;

			var directionToTarget = targetTransform.position - agentTransform.position;
			var distanceToTarget = directionToTarget.With(y: 0).magnitude; //TODO: currently filters out y axis
			var normalizedDistance = Mathf.Clamp01(distanceToTarget / maxDistance);

			var utility = Mathf.Clamp01(curve.Evaluate(normalizedDistance));
			return utility;
		}

		private void Reset()
		{
			curve = new AnimationCurve(
				new Keyframe(0f, 1f),
				new Keyframe(1f, 0f)
			);
		}
	}
}
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
		public AnimationCurve curve;
		
		public override float Evaluate(Context context)
		{
			if (!context.sensor.targetTags.Contains(targetTag))
			{
				context.sensor.targetTags.Add(targetTag);
			}

			var targetTransform = context.sensor.GetClosestTarget(targetTag);
			if (!targetTransform) return 0f;

			var agentTransform = context.agent.transform;

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
using UnityEngine;

namespace Source.AI.UtilityAI
{
	[CreateAssetMenu(menuName = "UtilityAI/Actions/MoveToTarget")]
	public class MoveToTargetAction : AIAction
	{
		public override void Initialize(Context context)
		{
			context.sensor.targetTags.Add(targetTag);
		}

		public override void Execute(Context context)
		{
			var target = context.sensor.GetClosestTarget(targetTag);
			if (target == null) return;

			context.agent.SetDestination(target.position);
		}
	}
}
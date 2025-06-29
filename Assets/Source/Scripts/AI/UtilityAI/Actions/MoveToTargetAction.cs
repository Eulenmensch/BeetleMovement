using UnityEngine;

namespace Source.AI.UtilityAI
{
	[CreateAssetMenu(menuName = "UtilityAI/Actions/MoveToTarget")]
	public class MoveToTargetAction : AIAction
	{
		public override void Execute(Context context)
		{
			var target = context.sensor.GetClosestTarget(targetTag);
			if (!target) return;

			context.agent.SetDestination(target.position);
		}
	}
}
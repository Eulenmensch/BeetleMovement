using UnityEngine;

namespace Source.AI.UtilityAI
{
	[CreateAssetMenu(menuName = "UtilityAI/Actions/MoveToTarget")]
	public class MoveToTargetAction : AIAction
	{
		public string targetTag;
		public override void Execute(Brain brain, IBlackboard blackboard)
		{
			var target = brain.sensor.GetClosestTarget(targetTag);
			if (!target) return;

			brain.agent.SetDestination(target.position);
		}
	}
}
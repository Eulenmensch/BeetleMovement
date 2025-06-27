using Source.Utils;
using UnityEngine;

namespace Source.AI.UtilityAI
{
	[CreateAssetMenu(menuName = "UtilityAI/Actions/MoveAwayFromTarget")]
	public class MoveAwayFromTargetAction : AIAction
	{
		public override void Initialize(Context context)
		{
			context.sensor.targetTags.Add(targetTag);
		}
		public override void Execute(Context context)
		{
			var target = context.sensor.GetClosestTarget(targetTag);
			if (!target) return;

			var enemyPosition = context.sensor.GetClosestTarget(targetTag).position;
			var agentPosition = context.agent.transform.position;
			//make the agent run further if the enemy is closer
			var fleeDistance = context.sensor.detectionRadius - Vector3.Distance(enemyPosition, agentPosition);
			var fleeDirection = (agentPosition - enemyPosition).With(y:0).normalized; //TODO: ignores y axis
			context.agent.SetDestination(agentPosition + fleeDirection * fleeDistance);
		}
	}
}
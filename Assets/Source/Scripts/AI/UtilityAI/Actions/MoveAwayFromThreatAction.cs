using Source.Utils;
using UnityEngine;

namespace Source.AI.UtilityAI
{
	[CreateAssetMenu(menuName = "UtilityAI/Actions/MoveAwayFromThreat")]
	public class MoveAwayFromThreatAction : AIAction
	{
		public float moveDistanceMin = 5f;
		public float moveDistanceMax = 10f;
		public override void Execute(Brain brain, IBlackboard blackboard)
		{
			if (!blackboard.Has(InsectKeys.NearestThreat)) return;
			var threat = blackboard.Get<Transform>(InsectKeys.NearestThreat);
			if (!threat) return;

			var agentPosition = brain.agent.transform.position;
			//make the agent run further if the enemy is closer
			var fleeDistance = Random.Range(moveDistanceMin, moveDistanceMax);
			var fleeDirection = blackboard.Get<Vector3>(AphidKeys.ThreatDirection);
			brain.agent.SetDestination(agentPosition + fleeDirection * fleeDistance);
		}
	}
}
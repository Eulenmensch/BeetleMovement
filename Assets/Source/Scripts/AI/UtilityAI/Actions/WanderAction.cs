using UnityEngine;
using UnityEngine.AI;

namespace Source.AI.UtilityAI
{
	[CreateAssetMenu(menuName = "UtilityAI/Actions/Wander")]
	public class WanderAction : AIAction
	{
		public float repathDistance;
		public float backtrackChance = 50f;
		public float wanderRadius = 10f;
		public float wanderAngleMax = 90f;
		private Vector3 targetPosition;
		public override void Execute(Brain brain, IBlackboard blackboard)
		{
			var agentPosition = brain.agent.transform.position;
			if (Vector3.Distance(agentPosition, brain.agent.destination) <= repathDistance)
			{
				brain.agent.SetDestination(GetRandomDestination(agentPosition, brain));
			}
		}

		private Vector3 GetRandomDestination(Vector3 agentPosition, Brain brain)
		{
			var randomPoint = Random.insideUnitSphere * wanderRadius;
			randomPoint += agentPosition;
			NavMeshHit hit;
			NavMesh.SamplePosition(randomPoint, out hit, wanderRadius, 1);
			var sampledPosition = hit.position;
				
			var direction = agentPosition - sampledPosition;
			var angleToAgentForward = Vector3.SignedAngle(brain.agent.transform.forward, direction, brain.agent.transform.up);

			var roll = Random.value;
			if (roll > backtrackChance)
			{
				return sampledPosition;
			}
			
			if (angleToAgentForward > wanderAngleMax)
			{
				GetRandomDestination(agentPosition, brain);
			}
			
			return sampledPosition;
		}
	}
}
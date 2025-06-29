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
		public override void Execute(Context context)
		{
			var agentPosition = context.agent.transform.position;
			if (Vector3.Distance(agentPosition, context.agent.destination) <= repathDistance)
			{
				context.agent.SetDestination(GetRandomDestination(agentPosition, context));
			}
		}

		private Vector3 GetRandomDestination(Vector3 agentPosition, Context context)
		{
			var randomPoint = Random.insideUnitSphere * wanderRadius;
			randomPoint += agentPosition;
			NavMeshHit hit;
			NavMesh.SamplePosition(randomPoint, out hit, wanderRadius, 1);
			var sampledPosition = hit.position;
				
			var direction = agentPosition - sampledPosition;
			var angleToAgentForward = Vector3.SignedAngle(context.agent.transform.forward, direction, context.agent.transform.up);

			var roll = Random.value;
			if (roll > backtrackChance)
			{
				return sampledPosition;
			}
			
			if (angleToAgentForward > wanderAngleMax)
			{
				GetRandomDestination(agentPosition, context);
			}
			
			return sampledPosition;
		}
	}
}
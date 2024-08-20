using UnityEngine;

namespace Source.Steering
{
	public class CohesionSteeringBehavior : SteeringBehavior
	{
		[SerializeField] private float viewAngle = 90f;
		private Transform[] targets;

		private void Start()
		{
			// GetOtherAgents(targets);
			SteeringController[] agents = FindObjectsByType<SteeringController>(FindObjectsSortMode.None);
			targets = new Transform[agents.Length - 1];
			int count = 0;
			foreach (var agent in agents)
			{
				if (agent.gameObject != gameObject)
				{
					targets[count] = agent.transform;
					count++;
				}
			}
		}

		public override SteeringData GetSteering(SteeringController steeringController)
		{
			SteeringData steeringData = new SteeringData();
			Vector3 centerOfMass = Vector3.zero;
			int count = 0;
			foreach (var target in targets)
			{
				Vector3 targetDir = target.position - transform.position;
				if (Vector3.Angle(targetDir, transform.forward) < viewAngle)
				{
					centerOfMass += target.position;
					count++;
				}
			}

			//if any agents are in the view cone
			if (count > 0)
			{
				centerOfMass = centerOfMass / count;
				steeringData.Linear = centerOfMass - transform.position;
				steeringData.Linear.Normalize();
				steeringData.Linear *= steeringController.MaxAcceleration;
			}
			
			return steeringData;
		}
	}
}
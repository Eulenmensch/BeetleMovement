using UnityEngine;

namespace Source.Steering
{
	public class AlignSteeringBehavior : SteeringBehavior
	{
		[SerializeField] private float alignmentRadius = 10f;
		//TODO: reference to filter agent types

		private Transform[] targets;

		//TODO: Later, this needs to subscribe to events for when agents spawn/die
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
			steeringData.Linear = Vector3.zero;

			int count = 0;
			foreach (var target in targets)
			{
				Vector3 targetDir = target.position - transform.position;
				if (targetDir.magnitude < alignmentRadius)
				{
					steeringData.Linear += target.GetComponent<Rigidbody>().linearVelocity;
					count++;
				}
			}
			//if there are any agents is close enough
			if (count > 0)
			{
				steeringData.Linear /= count;
				if (steeringData.Linear.magnitude > steeringController.MaxAcceleration)
				{
					steeringData.Linear = steeringData.Linear.normalized * steeringController.MaxAcceleration;
				}
			}

			return steeringData;
		}
	}
}
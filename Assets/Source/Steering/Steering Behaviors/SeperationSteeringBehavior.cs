using UnityEngine;

namespace Source.Steering
{
	public class SeperationSteeringBehavior : SteeringBehavior
	{
		[SerializeField] private float seperationRadius;
		[SerializeField] private float decayCoefficient;

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

			foreach (var target in targets)
			{
				Vector3 direction = target.transform.position - transform.position;
				float distance = direction.magnitude;
				if (distance < seperationRadius)
				{
					float strength = Mathf.Min(decayCoefficient / (distance * distance),
						steeringController.MaxAcceleration);
					direction.Normalize();
					steeringData.Linear = direction * strength;
				}
			}
			return steeringData;
		}
	}
}
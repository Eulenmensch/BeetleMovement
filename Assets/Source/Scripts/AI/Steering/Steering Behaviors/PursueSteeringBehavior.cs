using UnityEngine;

namespace Source.AI.Steering
{
	public class PursueSteeringBehavior : SteeringBehavior
	{
		[SerializeField] private GameObject target;
		[SerializeField] private float maxPrediction = 2f;
		
		public override SteeringData GetSteering(SteeringController steeringController)
		{
			SteeringData steeringData = new SteeringData();
			Vector3 direction = target.transform.position - transform.position;
			float distance = direction.magnitude;
			float speed = steeringController.RigidB.velocity.magnitude;
			
			float prediction;
			if (speed <= distance / maxPrediction)
			{
				prediction = maxPrediction;
			}
			else
			{
				prediction = distance / speed;
			}

			Vector3 predictedTarget =
				target.transform.position + target.GetComponent<Rigidbody>().linearVelocity * prediction;

			steeringData.Linear = predictedTarget - transform.position;
			steeringData.Linear.Normalize();
			steeringData.Linear *= steeringController.MaxAcceleration;
			steeringData.Angular = 0;

			return steeringData;
		}
	}
}
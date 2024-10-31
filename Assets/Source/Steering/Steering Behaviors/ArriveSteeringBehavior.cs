using UnityEngine;

namespace Source.Steering
{
	public class ArriveSteeringBehavior : SteeringBehavior
	{
		[SerializeField] private Transform target;
		[SerializeField] private float targetRadius;
		[SerializeField] private float slowRadius;

		public override SteeringData GetSteering(SteeringController steeringController)
		{
			SteeringData steeringData = new SteeringData();
			Vector3 direction = target.position - transform.position;
			float distance = direction.magnitude;
			if (distance < targetRadius)
			{
				steeringController.RigidB.velocity = Vector3.zero;
				return steeringData;
			}

			float targetSpeed;
			
			if (distance > slowRadius)
			{
				targetSpeed = steeringController.MaxAcceleration;
			}
			else
			{
				targetSpeed = steeringController.MaxAcceleration * (distance / slowRadius);
			}

			Vector3 targetVelocity = direction;
			targetVelocity.Normalize();
			targetVelocity *= targetSpeed;
			steeringData.Linear = targetVelocity - (Vector3)steeringController.RigidB.velocity;
			if (steeringData.Linear.magnitude > steeringController.MaxAcceleration)
			{
				steeringData.Linear.Normalize();
				steeringData.Linear *= steeringController.MaxAcceleration;
			}

			steeringData.Angular = 0;
			return steeringData;
		}
	}
}
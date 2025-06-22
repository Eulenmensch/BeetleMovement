using UnityEngine;

namespace Source.AI.Steering
{
	public class SeekSteeringBehavior : SteeringBehavior
	{
		[SerializeField] private GameObject target;
		
		public override SteeringData GetSteering(SteeringController steeringController)
		{
			SteeringData steeringData = new SteeringData();
			steeringData.Linear = target.transform.position - transform.position;
			steeringData.Linear.Normalize();
			steeringData.Linear *= steeringController.MaxAcceleration;
			steeringData.Angular = 0;
			return steeringData;
		}
	}
}
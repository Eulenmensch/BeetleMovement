using UnityEngine;

namespace Source.Steering
{
	public abstract class SteeringBehavior : MonoBehaviour
	{
		[field: SerializeField] public float Weight { get; set; }

		public abstract SteeringData GetSteering(SteeringController steeringController);
		
		protected void GetOtherAgents(Transform[] targets)
		{
			//FIXME: Should probably be a List or other form of collection to forgo the count variable
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
	}
}
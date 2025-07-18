using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Source.AI.UtilityAI
{
	[RequireComponent(typeof(NavMeshAgent), typeof(Sensor))]
	public abstract class Brain : MonoBehaviour
	{
		public List<AIAction> actions;
		public NavMeshAgent agent;
		public Sensor sensor;
		public IBlackboard agentBlackboard;

		private void Awake()
		{
			agentBlackboard = InitializeAgentBb();
			foreach (var action in actions)
			{
				action.Initialize(agentBlackboard);
			}
			agent = GetComponent<NavMeshAgent>();
			sensor = GetComponent<Sensor>();
			sensor.Brain = this;
		}

		protected abstract IBlackboard InitializeAgentBb();
	

		private void Update()
		{
			AIAction bestAction = null;
			float highestUtility = float.MinValue;

			foreach (var action in actions)
			{
				float utility = action.CalculateUtility(this, agentBlackboard);
				if (utility > highestUtility)
				{
					highestUtility = utility;
					bestAction = action;
				}
			}

			if (bestAction)
			{
				bestAction.Execute(this, agentBlackboard);
			}
		}
	}
}
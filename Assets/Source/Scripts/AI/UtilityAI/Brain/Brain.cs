using System.Collections.Generic;
using Source.Beetles.Stats;
using UnityEngine;
using UnityEngine.AI;

namespace Source.AI.UtilityAI
{
	[RequireComponent(typeof(NavMeshAgent), typeof(Sensor))]
	public class Brain : MonoBehaviour
	{
		public List<AIAction> actions;
		public Context context;

		public Hunger hunger;

		private void Awake()
		{
			context = new Context(this);

			foreach (var action in actions)
			{
				action.Initialize(context);
			}
		}

		private void Update()
		{
			UpdateContext();

			AIAction bestAction = null;
			float highestUtility = float.MinValue;

			foreach (var action in actions)
			{
				float utility = action.CalculateUtility(context);
				if (utility > highestUtility)
				{
					highestUtility = utility;
					bestAction = action;
				}
			}

			if (bestAction)
			{
				bestAction.Execute(context);
			}
		}

		private void UpdateContext()
		{
			context.SetData("hunger", hunger.NormalizedHunger);
		}
	}
}
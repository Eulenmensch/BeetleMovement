using System;
using Source.Beetles.Stats;
using UnityEngine;

namespace Source.AI.UtilityAI
{
	[RequireComponent(typeof(Hunger))]
	public class AphidBrain : Brain, IPerceivable
	{
		public string PerceptType => "Aphid";

		public Percept GetPercept()
		{
			return new Percept(PerceptType, transform.position);
		}

		private Hunger hunger;

		private void Start()
		{
			hunger = GetComponent<Hunger>();
		}

		protected override IBlackboard InitializeAgentBb()
		{
			agentBlackboard = new AphidBlackboard();
			agentBlackboard.Set(InsectKeys.Hunger, hunger);
			agentBlackboard.Set(AphidKeys.ThreatLevel, 0f);
			return agentBlackboard;
		}
	}
}
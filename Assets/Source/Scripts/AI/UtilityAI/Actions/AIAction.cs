using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Source.AI.UtilityAI
{
	public abstract class AIAction : SerializedScriptableObject
	{
		[InfoBox("Weights are clamped between 0 and 1")]
		[DictionaryDrawerSettings(KeyLabel = "Consideration", ValueLabel = "Weight")]
		public Dictionary<Consideration, float> considerations;
		public virtual void Initialize(IBlackboard blackboard){}

		public float CalculateUtility(Brain brain, IBlackboard blackboard)
		{
			float utility = 1f;
			foreach (var consideration in considerations.Keys)
			{
				if (considerations.TryGetValue(consideration, out float weight))
				{
					weight = Mathf.Clamp01(weight);
					var weightedUtility = consideration.Evaluate(brain, blackboard) * weight;
					utility *= weightedUtility;
				}
			}

			return utility;
		}

		public abstract void Execute(Brain brain, IBlackboard blackboard);
	}
}
using UnityEngine;

namespace Source.AI.UtilityAI
{
	public abstract class AIAction : ScriptableObject
	{
		public string targetTag;
		public Consideration consideration;

		public virtual void Initialize(Context context)
		{
			if (targetTag != "")
			{
				context.sensor.targetTags.Add(targetTag);
			}
		}
		
		public float CalculateUtility(Context context) => consideration.Evaluate(context);

		public abstract void Execute(Context context);
	}
}
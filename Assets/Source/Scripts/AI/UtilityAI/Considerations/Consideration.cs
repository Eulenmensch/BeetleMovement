using UnityEngine;

namespace Source.AI.UtilityAI
{
	public abstract class Consideration : ScriptableObject
	{
		public abstract float Evaluate(Context context);
	}
}
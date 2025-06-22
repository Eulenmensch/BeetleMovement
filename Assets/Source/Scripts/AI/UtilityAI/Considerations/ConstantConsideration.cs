using UnityEngine;

namespace Source.AI.UtilityAI
{
	[CreateAssetMenu(menuName = "UtilityAI/Considerations/Constant")]
	public class ConstantConsideration : Consideration
	{
		public float value;
		
		public override float Evaluate(Context context) => value;
	}
}
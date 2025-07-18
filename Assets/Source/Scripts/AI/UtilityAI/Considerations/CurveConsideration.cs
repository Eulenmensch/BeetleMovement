using Sirenix.OdinInspector;
using UnityEngine;

namespace Source.AI.UtilityAI
{
	[CreateAssetMenu(menuName = "UtilityAI/Considerations/Curve")]
	public class CurveConsideration : Consideration
	{
		[InfoBox("X axis = input value, y axis = utility")]
		public AnimationCurve curve;
		public string blackboardKey;
		
		public override float Evaluate(Brain brain, IBlackboard blackboard)
		{
			float inputValue = blackboard.Get<float>(blackboardKey);
			float utility = curve.Evaluate(inputValue);
			return Mathf.Clamp01(utility);
		}

		void Reset()
		{
			curve = new AnimationCurve(
				new Keyframe(0f, 1f),
				new Keyframe(1f, 0f)
			);
		}
	}
}
using UnityEngine;

namespace Source.AI.UtilityAI
{
	[CreateAssetMenu(menuName = "UtilityAI/Actions/Idle")]
	public class IdleAIAction : AIAction
	{
		public override void Execute(Context context)
		{
			context.agent.SetDestination(context.agent.transform.position);
		}
	}
}
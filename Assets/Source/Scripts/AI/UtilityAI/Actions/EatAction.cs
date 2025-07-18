using Source.Beetles.Stats;
using Source.Things;
using UnityEngine;

namespace Source.AI.UtilityAI
{
	[CreateAssetMenu(menuName = "UtilityAI/Actions/Eat")]
	public class EatAction : AIAction
	{
		public float consumptionPerSecond = 1;
		public override void Execute(Brain brain, IBlackboard blackboard)
		{
			var foodSource = blackboard.Get<FoodSource>(AphidKeys.NearestFood);
			if (!foodSource) return;

			var consumption = consumptionPerSecond * Time.deltaTime;

			foodSource.Drain(consumption);

			blackboard.Get<Hunger>(InsectKeys.Hunger).ReduceHunger(consumption);
		}
	}
}
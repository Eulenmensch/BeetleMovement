using Source.Things;
using UnityEngine;

namespace Source.AI.UtilityAI
{
	[CreateAssetMenu(menuName = "UtilityAI/Actions/Eat")]
	public class EatAction : AIAction
	{
		public float consumptionPerSecond = 1;
		public override void Execute(Context context)
		{
			var target = context.sensor.GetClosestTarget(targetTag);
			if (!target) return;

			var consumption = consumptionPerSecond * Time.deltaTime;
			
			if (target.TryGetComponent<FoodSource>(out var foodSource))
			{
				foodSource.Drain(consumption);
			}
			
			context.brain.hunger.ReduceHunger(consumption);
		}
	}
}
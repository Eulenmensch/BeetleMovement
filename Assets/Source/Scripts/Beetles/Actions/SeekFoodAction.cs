using Source.Things;
using UnityEngine;

namespace Source.Beetles.Actions
{
    // Drives the beetle toward the nearest available food source.
    // Score scales with hunger so starving beetles prioritise food over wandering.
    public class SeekFoodAction : BeetleAction
    {
        // Max score 1.5 at hunger = 1. Intentionally below Eat (2.0) so once the beetle
        // reaches food it switches to eating rather than circling.
        private const float MaxScore = 1.5f;

        public override bool IsValid(BeetleBlackboard board)
            => board.Get<FoodSource>(BK.NearestFood) != null
            && !board.Get<bool>(BK.AtFoodSource);

        public override float Score(BeetleBlackboard board)
            => board.Get<float>(BK.Hunger) * MaxScore;

        public override void Execute(BeetleBrain brain, BeetleBlackboard board)
        {
            var food = board.Get<FoodSource>(BK.NearestFood);
            if (food == null) return;
            brain.RequestPath(food.transform.position);
        }
    }
}

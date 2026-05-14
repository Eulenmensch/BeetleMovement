using UnityEngine;

namespace Source.Beetles.Actions
{
    // Predator only: pursues the nearest cattle beetle. Score mirrors SeekFoodAction.
    public class SeekPreyAction : BeetleAction
    {
        private const float MaxScore = 1.5f;

        public override bool IsValid(BeetleBlackboard board)
            => board.Get<Transform>(BK.NearestPrey) != null;

        public override float Score(BeetleBlackboard board)
            => board.Get<float>(BK.Hunger) * MaxScore;

        public override void Execute(BeetleBrain brain, BeetleBlackboard board)
        {
            var prey = board.Get<Transform>(BK.NearestPrey);
            if (prey == null) return;
            // Refresh path each tick so the beetle tracks a moving target.
            brain.ClearPath();
            brain.RequestPath(prey.position);
        }
    }
}

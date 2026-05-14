using Source.Things;

namespace Source.Beetles.Actions
{
    // Consumes food while the beetle is at a food source. Score scales with hunger;
    // max 2.0 so it beats pheromone-following (3.0) only once the beetle is very hungry
    // AND physically at the food — otherwise pheromones guide it there first.
    public class EatBeetleAction : BeetleAction
    {
        private readonly float _drainRate; // units drained from FoodSource per second
        private const    float MaxScore = 2.0f;

        public EatBeetleAction(float drainRate = 0.05f)
        {
            _drainRate = drainRate;
        }

        public override bool IsValid(BeetleBlackboard board)
            => board.Get<bool>(BK.AtFoodSource)
            && board.Get<FoodSource>(BK.NearestFood) != null;

        public override float Score(BeetleBlackboard board)
            => board.Get<float>(BK.Hunger) * MaxScore;

        public override void Execute(BeetleBrain brain, BeetleBlackboard board)
        {
            // Pass the tick interval so eating reduces hunger at the correct per-second rate
            // even though Execute is called once per AI tick, not once per frame.
            brain.Hunger.Eat(brain.UpdateInterval);

            var food = board.Get<FoodSource>(BK.NearestFood);
            food?.Drain(_drainRate * brain.UpdateInterval);
        }
    }
}

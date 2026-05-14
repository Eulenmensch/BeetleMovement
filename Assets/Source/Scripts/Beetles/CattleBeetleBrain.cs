using System.Collections.Generic;
using Source.Beetles.Actions;

namespace Source.Beetles
{
    // Cattle beetle: grazes food sources, follows pheromone trails, flees predators.
    // Add PheromoneEmitter to the same GameObject so it marks food locations for others.
    //
    // Utility priority at max inputs (score caps):
    //   Flee (10.0) > Pheromone (3.0) > Eat (2.0) > SeekFood (1.5) > Wander (0.3)
    public class CattleBeetleBrain : BeetleBrain
    {
        protected override bool IsPredator => false;

        protected override List<BeetleAction> CreateActions() => new()
        {
            new FleeAction(senseRadius: 8f, fleeDistance: 5f),
            new FollowPheromoneAction(followDistance: 5f),
            new EatBeetleAction(drainRate: 0.05f),
            new SeekFoodAction(),
            new WanderBeetleAction(wanderRadius: 6f),
        };
    }
}

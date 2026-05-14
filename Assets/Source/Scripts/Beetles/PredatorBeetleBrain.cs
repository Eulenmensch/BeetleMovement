using System.Collections.Generic;
using Source.Beetles.Actions;

namespace Source.Beetles
{
    // Predator beetle: hunts cattle beetles when hungry, wanders otherwise.
    // No flee, eat, or pheromone actions — predators ignore the pheromone layer.
    //
    // Utility priority at max inputs:
    //   SeekPrey (1.5) > Wander (0.3)
    public class PredatorBeetleBrain : BeetleBrain
    {
        protected override bool IsPredator => true;

        protected override List<BeetleAction> CreateActions() => new()
        {
            new SeekPreyAction(),
            new WanderBeetleAction(wanderRadius: 8f),
        };
    }
}

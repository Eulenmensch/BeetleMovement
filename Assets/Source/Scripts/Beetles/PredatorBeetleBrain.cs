using System.Collections.Generic;
using Source.Beetles.Actions;
using UnityEngine;

namespace Source.Beetles
{
    // Predator beetle: hunts and eats cattle beetles when hungry, wanders otherwise.
    // No flee or pheromone actions — predators ignore the pheromone layer.
    //
    // Utility priority at max inputs:
    //   EatPrey (2.0) > SeekPrey (1.5) > Wander (0.3)
    public class PredatorBeetleBrain : BeetleBrain
    {
        [SerializeField] private float damagePerSecond = 3f;
        [SerializeField] private float hungerRecoveryPerSecond = 0.2f;
        
        protected override bool IsPredator => true;

        protected override List<BeetleAction> CreateActions() => new()
        {
            new EatPreyAction(damagePerSecond, hungerRecoveryPerSecond),
            new SeekPreyAction(),
            new WanderBeetleAction(wanderRadius: 8f),
        };
    }
}

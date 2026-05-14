namespace Source.Beetles.Actions
{
    // Abstract base for all beetle utility-AI actions.
    // Concrete actions are plain C# classes instantiated by each brain type —
    // no ScriptableObject overhead needed for a prototype.
    public abstract class BeetleAction
    {
        // True when this action's preconditions are met (e.g. at a food source for Eat).
        public abstract bool IsValid(BeetleBlackboard board);

        // Utility score. Higher wins. Tune constants so at max inputs: flee > pheromone > eat > wander.
        public abstract float Score(BeetleBlackboard board);

        // Perform the action — usually sets a path destination or modifies a stat.
        public abstract void Execute(BeetleBrain brain, BeetleBlackboard board);
    }
}

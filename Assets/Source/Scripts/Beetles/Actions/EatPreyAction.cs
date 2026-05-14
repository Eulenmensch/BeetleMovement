namespace Source.Beetles.Actions
{
    // Predator only: deals damage to a cattle beetle that is within eat range,
    // recovering the predator's hunger proportional to damage dealt.
    // Score mirrors EatBeetleAction (2.0) so it beats SeekPrey (1.5) once the
    // predator is actually on top of its target.
    public class EatPreyAction : BeetleAction
    {
        private readonly float _damagePerSecond;
        private readonly float _hungerRecoveryPerSecond;
        private const    float MaxScore = 2.0f;

        public EatPreyAction(float damagePerSecond = 30f, float hungerRecoveryPerSecond = 0.15f)
        {
            _damagePerSecond         = damagePerSecond;
            _hungerRecoveryPerSecond = hungerRecoveryPerSecond;
        }

        public override bool IsValid(BeetleBlackboard board)
            => board.Get<bool>(BK.AtPrey)
            && board.Get<BeetleHealth>(BK.NearestPreyHealth) != null;

        public override float Score(BeetleBlackboard board)
            => board.Get<float>(BK.Hunger) * MaxScore;

        public override void Execute(BeetleBrain brain, BeetleBlackboard board)
        {
            var preyHealth = board.Get<BeetleHealth>(BK.NearestPreyHealth);
            if (preyHealth == null) return;

            float dt = brain.UpdateInterval;
            preyHealth.TakeDamage(_damagePerSecond * dt);
            brain.Hunger.Restore(_hungerRecoveryPerSecond * dt);
        }
    }
}

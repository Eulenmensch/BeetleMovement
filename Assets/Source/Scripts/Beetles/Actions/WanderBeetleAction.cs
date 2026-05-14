using UnityEngine;

namespace Source.Beetles.Actions
{
    // Low-constant baseline; always valid so the beetle has something to do when all other
    // actions are either invalid or score below 0.3.
    public class WanderBeetleAction : BeetleAction
    {
        private readonly float _wanderRadius;
        private float          _nextWanderTime;

        public WanderBeetleAction(float wanderRadius = 6f)
        {
            _wanderRadius = wanderRadius;
        }

        public override bool  IsValid(BeetleBlackboard board) => true;
        public override float Score(BeetleBlackboard board)   => 0.3f;

        public override void Execute(BeetleBrain brain, BeetleBlackboard board)
        {
            // Only pick a new wander point occasionally so the beetle doesn't jitter.
            if (Time.time < _nextWanderTime) return;
            _nextWanderTime = Time.time + Random.Range(3f, 6f);

            // Random direction on the surface tangent plane, projected onto a point at wander radius.
            Vector3 randomOffset = Random.insideUnitSphere;
            randomOffset = Vector3.ProjectOnPlane(randomOffset, brain.SurfaceNormal).normalized * _wanderRadius;
            brain.RequestPath(brain.transform.position + randomOffset);
        }
    }
}

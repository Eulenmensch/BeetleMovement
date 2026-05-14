using UnityEngine;

namespace Source.Beetles.Actions
{
    // Cattle beetle only: flees from the nearest predator. At full proximity this scores 10.0,
    // ensuring it overrides every other behaviour (pheromones cap at 3.0, eat at 2.0).
    public class FleeAction : BeetleAction
    {
        private readonly float _fleeDistance;
        private readonly float _senseRadius;  // should match BeetleSensor.senseRadius
        private const    float MaxScore = 10.0f;

        public FleeAction(float senseRadius = 8f, float fleeDistance = 5f)
        {
            _senseRadius  = senseRadius;
            _fleeDistance = fleeDistance;
        }

        public override bool IsValid(BeetleBlackboard board)
            => board.Get<Transform>(BK.NearestThreat) != null;

        public override float Score(BeetleBlackboard board)
        {
            float dist      = board.Get<float>(BK.ThreatDistance, float.MaxValue);
            float proximity = Mathf.Clamp01(1f - dist / _senseRadius);
            return proximity * MaxScore;
        }

        public override void Execute(BeetleBrain brain, BeetleBlackboard board)
        {
            var threat = board.Get<Transform>(BK.NearestThreat);
            if (threat == null) return;

            // Flee directly away from the threat along the surface tangent plane.
            Vector3 awayDir = Vector3.ProjectOnPlane(
                (brain.transform.position - threat.position).normalized,
                brain.SurfaceNormal);

            // Force a fresh path so the beetle reacts immediately.
            brain.ClearPath();
            brain.RequestPath(brain.transform.position + awayDir * _fleeDistance);
        }
    }
}

using UnityEngine;

namespace Source.Beetles.Actions
{
    // Cattle beetle only: follows the pheromone gradient toward food marked by other beetles.
    // Score caps at 3.0 (pheromoneStrength = 1.0), placing it between flee (10.0) and eat (2.0).
    public class FollowPheromoneAction : BeetleAction
    {
        private readonly float _followDistance;
        private const    float MaxScore = 3.0f;

        public FollowPheromoneAction(float followDistance = 5f)
        {
            _followDistance = followDistance;
        }

        public override bool IsValid(BeetleBlackboard board)
            => board.Get<float>(BK.PheromoneStrength) > 0.05f; // ignore negligible traces

        public override float Score(BeetleBlackboard board)
            => board.Get<float>(BK.PheromoneStrength) * MaxScore;

        public override void Execute(BeetleBrain brain, BeetleBlackboard board)
        {
            Vector3 dir = board.Get<Vector3>(BK.PheromoneDir);
            if (dir.sqrMagnitude < 0.001f) return;

            dir = Vector3.ProjectOnPlane(dir, brain.SurfaceNormal).normalized;
            brain.RequestPath(brain.transform.position + dir * _followDistance);
        }
    }
}

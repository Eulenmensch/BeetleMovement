using UnityEngine;
using Source.Things;

namespace Source.Beetles
{
    // Scans the environment via OverlapSphere and writes results into BeetleBlackboard.
    // Called by BeetleBrain once per AI tick (not every frame) to amortise physics cost.
    public class BeetleSensor : MonoBehaviour
    {
        [SerializeField] private float      senseRadius   = 8f;
        [SerializeField] private float      eatRadius     = 0.8f;  // distance to count as "at food source"
        [SerializeField] private LayerMask  foodLayer      = ~0;
        [SerializeField] private LayerMask  beetleLayer    = ~0;
        [SerializeField] private LayerMask  pheromoneLayer = ~0;
        [SerializeField] private string     predatorTag   = "Predator";
        [SerializeField] private string     preyTag       = "CattleBeetle";

        private BeetleBlackboard _board;
        private bool             _isPredator;

        private readonly Collider[] _buffer = new Collider[32];

        public void Init(BeetleBlackboard board, bool isPredator)
        {
            _board      = board;
            _isPredator = isPredator;
        }

        // Run one full sense pass. Called by BeetleBrain on its staggered tick.
        public void Sense()
        {
            SenseFood();
            SensePheromones();

            if (_isPredator) SensePrey();
            else             SenseThreat();
        }

        private void SenseFood()
        {
            int count = Physics.OverlapSphereNonAlloc(transform.position, senseRadius, _buffer, foodLayer);
            FoodSource best     = null;
            float      bestDist = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                if (!_buffer[i].TryGetComponent(out FoodSource fs)) continue;
                if (fs.CurrentAmount <= 0f) continue; // skip exhausted sources

                float d = (_buffer[i].transform.position - transform.position).sqrMagnitude;
                if (d < bestDist) { bestDist = d; best = fs; }
            }

            _board.Set(BK.NearestFood,  best);
            _board.Set(BK.AtFoodSource, best != null && Mathf.Sqrt(bestDist) < eatRadius);
        }

        private void SenseThreat()
        {
            int       count       = Physics.OverlapSphereNonAlloc(transform.position, senseRadius, _buffer, beetleLayer);
            Transform nearest     = null;
            float     nearestDist = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                if (!_buffer[i].CompareTag(predatorTag)) continue;
                float d = (_buffer[i].transform.position - transform.position).sqrMagnitude;
                if (d < nearestDist) { nearestDist = d; nearest = _buffer[i].transform; }
            }

            _board.Set(BK.NearestThreat,  nearest);
            _board.Set(BK.ThreatDistance, nearest != null ? Mathf.Sqrt(nearestDist) : float.MaxValue);
        }

        private void SensePrey()
        {
            int       count       = Physics.OverlapSphereNonAlloc(transform.position, senseRadius, _buffer, beetleLayer);
            Transform nearest     = null;
            float     nearestDist = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                if (!_buffer[i].CompareTag(preyTag)) continue;
                float d = (_buffer[i].transform.position - transform.position).sqrMagnitude;
                if (d < nearestDist) { nearestDist = d; nearest = _buffer[i].transform; }
            }

            _board.Set(BK.NearestPrey, nearest);
        }

        private void SensePheromones()
        {
            int     count         = Physics.OverlapSphereNonAlloc(transform.position, senseRadius, _buffer, pheromoneLayer);
            float   totalStrength = 0f;
            Vector3 weightedDir   = Vector3.zero;

            for (int i = 0; i < count; i++)
            {
                if (!_buffer[i].TryGetComponent(out Pheromone p)) continue;
                if (p.Emitter == gameObject) continue; // ignore own pheromones
                totalStrength += p.Strength;
                weightedDir   += (_buffer[i].transform.position - transform.position).normalized * p.Strength;
            }

            _board.Set(BK.PheromoneStrength, Mathf.Clamp01(totalStrength));
            _board.Set(BK.PheromoneDir,
                       weightedDir.sqrMagnitude > 0.001f ? weightedDir.normalized : Vector3.zero);
        }
    }
}

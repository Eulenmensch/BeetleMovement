using UnityEngine;

namespace Source.Beetles
{
    // Periodically spawns a Pheromone prefab at the beetle's current position.
    // Attach to cattle beetles only; disable or remove from predators.
    public class PheromoneEmitter : MonoBehaviour
    {
        [SerializeField] private GameObject pheromonePrefab; // prefab with Pheromone + SphereCollider (trigger)
        [SerializeField] private float emitInterval = 1.5f;

        private float _nextEmitTime;

        private void Update()
        {
            if (Time.time < _nextEmitTime) return;
            _nextEmitTime = Time.time + emitInterval;

            if (pheromonePrefab == null) return;
            var go = Instantiate(pheromonePrefab, transform.position, Quaternion.identity);
            if (go.TryGetComponent(out Pheromone p))
                p.Init(gameObject);
        }
    }
}

using Sirenix.OdinInspector;
using UnityEngine;

namespace Source.Beetles
{
    // A fading trigger volume emitted by cattle beetles to mark food locations.
    // Place a SphereCollider (Is Trigger) on the same GameObject, set to the pheromone layer.
    public class Pheromone : MonoBehaviour
    {
        
        [SerializeField, InfoBox("In Seconds")] private float lifetime = 8f;

        public float      Strength { get; private set; } = 1f;
        public GameObject Emitter  { get; private set; }

        // Called by PheromoneEmitter immediately after instantiation.
        public void Init(GameObject emitter) => Emitter = emitter;

        private float _elapsed;

        private void Update()
        {
            _elapsed += Time.deltaTime;
            Strength  = Mathf.Clamp01(1f - _elapsed / lifetime);

            if (_elapsed >= lifetime)
                Destroy(gameObject);
        }
    }
}

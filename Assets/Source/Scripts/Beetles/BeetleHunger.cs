using Sirenix.OdinInspector;
using UnityEngine;

namespace Source.Beetles
{
    // Hunger rises passively over time. At 1.0 the beetle becomes immobilised (not destroyed)
    // so it remains visible for debugging. Actions call Eat() while the beetle is feeding.
    public class BeetleHunger : MonoBehaviour
    {
        [InfoBox("Hunger maxes out at 1.0, so these are decimal percentages")]
        [InfoBox("At 1 hunger, the beetle becomes immobile")]
        [SerializeField] private float hungerRate = 0.02f; // per second, passive increase
        [SerializeField] private float eatRate    = 0.15f; // per second while actively eating

        public float Value        { get; private set; }
        public bool  IsMaxHunger  => Value >= 1f;

        private void Update()
        {
            Value = Mathf.Clamp01(Value + hungerRate * Time.deltaTime);
        }

        // Pass the duration over which eating occurred (usually the AI tick interval).
        public void Eat(float duration) => Value = Mathf.Clamp01(Value - eatRate * duration);
    }
}

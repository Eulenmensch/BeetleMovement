using UnityEngine;

namespace Source.Beetles
{
    public class BeetleHealth : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100f;

        public float Value    { get; private set; }
        public bool  IsDead   => Value <= 0f;
        public float Fraction => Value / maxHealth;

        private void Awake() => Value = maxHealth;

        public void TakeDamage(float amount)
        {
            Value = Mathf.Max(0f, Value - amount);
            if (Value <= 0f)
                Destroy(gameObject);
        }
    }
}

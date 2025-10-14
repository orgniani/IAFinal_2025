using System;
using UnityEngine;

namespace Damage
{
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth = 3f;
        [SerializeField] private bool shouldDespawn = true;
        private float _currentHealth;

        public event Action OnHit = delegate { };
        public event Action OnDie = delegate { };

        public bool IsAlive => _currentHealth > 0f;

        private void Awake()
        {
            _currentHealth = maxHealth;
        }

        public void ApplyDamage(float amount)
        {
            _currentHealth -= amount;
            _currentHealth = Mathf.Clamp(_currentHealth, 0f, maxHealth);

            OnHit?.Invoke();

            if (_currentHealth <= 0f)
                Die();
        }

        private void Die()
        {
            OnDie?.Invoke();

            Debug.Log($"{gameObject.name} died.");

            //TODO: delete later, just for testing
            if (shouldDespawn) 
                gameObject.SetActive(false);
        }
    }
}

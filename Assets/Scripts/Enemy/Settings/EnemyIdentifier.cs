using System;
using UnityEngine;
using Damage;

namespace Enemy
{
    [RequireComponent(typeof(HealthController))]
    public class EnemyIdentifier : MonoBehaviour
    {
        [SerializeField] private EnemyType type = EnemyType.None;

        private HealthController _health;
        public EnemyType Type => type;
        public event Action<EnemyType> OnDeath = delegate { };

        private void Awake()
        {
            _health = GetComponent<HealthController>();
        }

        private void OnEnable()
        {
            _health.OnDie += HandleDeath;
        }

        private void OnDisable()
        {
            _health.OnDie -= HandleDeath;
        }

        private void HandleDeath()
        {
            OnDeath.Invoke(type);
        }
    }
}

using System;
using System.Collections;
using UnityEngine;

namespace Damage
{
    public class HealthController : MonoBehaviour, IDamageable
    {
        [Header("General")]
        [SerializeField] private float maxHealth = 3f;
        [SerializeField] private bool shouldDespawn = true;
        [SerializeField] private bool canRespawn = true;

        [Header("Timers")]
        [SerializeField] private float deathCooldown = 3f;
        [SerializeField] private float aggroDuration = 60f;

        private float _currentHealth;
        private Vector3 _spawnPosition;

        private bool _aggroLocked;
        private Coroutine _aggroRoutine;

        public event Action OnHit = delegate { };
        public event Action OnDie = delegate { };
        public event Action OnRespawn = delegate { };

        public bool IsAlive => _currentHealth > 0f;
        public bool AggroLocked => _aggroLocked;
        public float Health => _currentHealth;
        public float MaxHealth => maxHealth;

        private void OnEnable()
        {
            _spawnPosition = transform.position;
            _currentHealth = maxHealth;
            _aggroLocked = false;
            _aggroRoutine = null;
        }

        public void ApplyDamage(float amount)
        {
            _currentHealth -= amount;
            _currentHealth = Mathf.Clamp(_currentHealth, 0f, maxHealth);

            _aggroLocked = true;
            RestartAggroCoroutine();

            OnHit?.Invoke();

            if (_currentHealth <= 0f)
                Die();
        }

        private void RestartAggroCoroutine()
        {
            if (_aggroRoutine != null)
                StopCoroutine(_aggroRoutine);

            _aggroRoutine = StartCoroutine(AggroCountdown());
        }

        private IEnumerator AggroCountdown()
        {
            yield return new WaitForSeconds(aggroDuration);
            ClearAggroLock();
        }

        public void ClearAggroLock()
        {
            _aggroLocked = false;

            if (_aggroRoutine != null)
            {
                StopCoroutine(_aggroRoutine);
                _aggroRoutine = null;
            }
        }

        private void Die()
        {
            OnDie?.Invoke();
            Debug.Log($"{gameObject.name} died.");

            if (canRespawn)
                RespawnManager.Instance.ScheduleRespawn(this, deathCooldown);

            if (shouldDespawn)
                gameObject.SetActive(false);
        }

        public void Respawn()
        {
            transform.position = _spawnPosition;
            _currentHealth = maxHealth;
            _aggroLocked = false;

            if (_aggroRoutine != null)
            {
                StopCoroutine(_aggroRoutine);
                _aggroRoutine = null;
            }

            gameObject.SetActive(true);
            OnRespawn?.Invoke();

            Debug.Log($"{gameObject.name} respawned.");
        }
    }
}

using UnityEngine;

namespace Damage
{
    public class Bullet : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float speed = 20f;
        [SerializeField] private float lifetime = 2f;
        [SerializeField] private float damage = 1f;

        private float _lifeTimer;
        private Vector3 _direction;

        private void OnEnable()
        {
            _lifeTimer = lifetime;
        }

        private void Update()
        {
            transform.Translate(_direction * speed * Time.deltaTime, Space.World);

            _lifeTimer -= Time.deltaTime;
            if (_lifeTimer <= 0f)
                gameObject.SetActive(false);
        }

        public void SetDirection(Vector3 direction)
        {
            _direction = direction.normalized;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.ApplyDamage(damage);
                gameObject.SetActive(false);
            }
        }
    }
}

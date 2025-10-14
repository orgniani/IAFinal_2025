using UnityEngine;

namespace Damage
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float speed = 20f;
        [SerializeField] private float lifetime = 2f;
        [SerializeField] private float damage = 1f;
        [SerializeField] private float downOffset = 0.05f;

        private float _lifeTimer;

        private void OnEnable()
        {
            _lifeTimer = lifetime;
        }

        private void Update()
        {
            Vector3 direction = Vector3.forward + Vector3.down * downOffset;
            transform.Translate(direction.normalized * speed * Time.deltaTime, Space.Self);

            _lifeTimer -= Time.deltaTime;

            if (_lifeTimer <= 0f)
                gameObject.SetActive(false);
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
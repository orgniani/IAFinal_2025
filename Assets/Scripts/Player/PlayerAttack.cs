using Damage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerAttack : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform firePoint;
        [SerializeField] private Bullet bulletPrefab;

        [Header("Pool Settings")]
        [SerializeField] private int minPoolSize = 5;
        [SerializeField] private int maxPoolSize = 30;

        [Header("Attack Settings")]
        [SerializeField] private float fireRate = 0.2f;
        [SerializeField] private float targetVerticalOffset = 1.2f;

        private PlayerController _player;
        private List<Bullet> _bulletPool;
        private Coroutine _shootingRoutine;
        private Transform _currentTarget;

        private void Awake()
        {
            _player = GetComponent<PlayerController>();
            InitializePool(minPoolSize);
        }

        private void OnEnable()
        {
            _player.OnShoot += HandleShoot;
        }

        private void OnDisable()
        {
            _player.OnShoot -= HandleShoot;
        }

        private void InitializePool(int size)
        {
            _bulletPool = new List<Bullet>();

            for (int i = 0; i < size; i++)
                AddBulletToPool();
        }

        private Bullet AddBulletToPool()
        {
            if (_bulletPool.Count >= maxPoolSize)
                return null;

            Bullet bullet = Instantiate(bulletPrefab);
            bullet.gameObject.SetActive(false);
            _bulletPool.Add(bullet);
            return bullet;
        }

        private void HandleShoot(bool shooting, Transform target)
        {
            _currentTarget = target;

            if (shooting)
            {
                if (_shootingRoutine == null)
                    _shootingRoutine = StartCoroutine(ShootRoutine());
            }
            else
            {
                if (_shootingRoutine != null)
                {
                    StopCoroutine(_shootingRoutine);
                    _shootingRoutine = null;
                }
            }
        }

        private IEnumerator ShootRoutine()
        {
            WaitForSeconds wait = new WaitForSeconds(fireRate);

            while (true)
            {
                SpawnBullet();
                yield return wait;
            }
        }

        private void SpawnBullet()
        {
            Bullet bullet = GetPooledBullet();
            if (bullet == null) return;

            bullet.transform.position = firePoint.position;

            Vector3 direction;
            if (_currentTarget != null)
            {
                Vector3 targetPos = _currentTarget.position + Vector3.up * targetVerticalOffset;
                direction = (targetPos - firePoint.position).normalized;
            }
            else
            {
                direction = firePoint.forward;
            }

            bullet.transform.rotation = Quaternion.LookRotation(direction);
            bullet.gameObject.SetActive(true);
            bullet.SetDirection(direction);
        }

        private Bullet GetPooledBullet()
        {
            foreach (var bullet in _bulletPool)
            {
                if (!bullet.gameObject.activeInHierarchy)
                    return bullet;
            }

            return AddBulletToPool();
        }
    }
}

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
        [SerializeField] private GameObject bulletPrefab;

        [Header("Pool Settings")]
        [SerializeField] private int minPoolSize = 5;
        [SerializeField] private int maxPoolSize = 30;

        [Header("Attack Settings")]
        [SerializeField] private float fireRate = 0.2f;

        private PlayerController _player;

        private List<GameObject> _bulletPool;
        private Coroutine _shootingRoutine;

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
            _bulletPool = new List<GameObject>();

            for (int i = 0; i < size; i++)
                AddBulletToPool();
        }

        private GameObject AddBulletToPool()
        {
            if (_bulletPool.Count >= maxPoolSize)
                return null;

            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            _bulletPool.Add(bullet);
            return bullet;
        }

        private void HandleShoot(bool shooting)
        {
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
            GameObject bullet = GetPooledBullet();
            if (bullet == null) return;

            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = firePoint.rotation;
            bullet.SetActive(true);
        }

        private GameObject GetPooledBullet()
        {
            foreach (var bullet in _bulletPool)
            {
                if (!bullet.activeInHierarchy)
                    return bullet;
            }

            return AddBulletToPool();
        }
    }
}

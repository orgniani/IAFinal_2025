using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Damage;
using Player;

namespace Enemy
{
    public class EnemyRangedAttackState : EnemyAttackStateBase
    {
        [Header("Ranged Attack Settings")]
        [SerializeField, Range(0f, 20f)] private float escapeRange = 3f;
        [SerializeField] private GameObject bulletPrefab;

        [Header("Pooling Settings")]
        [SerializeField] private int minPoolSize = 5;
        [SerializeField] private int maxPoolSize = 30;

        private List<GameObject> _bulletPool;
        private float _updateTimer;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            if (_bulletPool == null)
                InitializePool(minPoolSize);

            _agent.isStopped = false;
            _updateTimer = 0f;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_target == null || _damageableTarget == null)
                return;

            if (!_damageableTarget.IsAlive)
            {
                animator.SetTrigger(animationParameters.playerDiedTrigger);
                return;
            }

            float distance = Vector3.Distance(_agent.transform.position, _target.position);
            _updateTimer += Time.deltaTime;

            if (_updateTimer >= settings.updateInterval)
            {
                _updateTimer = 0f;
                Vector3 dirFromPlayer = (_agent.transform.position - _target.position).normalized;

                if (distance < escapeRange)
                {
                    animator.SetTrigger(animationParameters.playerTooCloseTrigger);
                    return;
                }

                else if (distance > settings.attackRange)
                {
                    _agent.SetDestination(_target.position);
                }
                else
                {
                    _agent.ResetPath();
                }
            }

            if (distance > settings.attackRange)
            {
                animator.SetTrigger(animationParameters.playerOutRangeTrigger);
                return;
            }

            if (distance <= settings.attackRange && distance >= escapeRange)
            {
                _cooldownTimer -= Time.deltaTime;
                if (_cooldownTimer <= 0f)
                {
                    _cooldownTimer = attackCooldown;
                    PerformAttack();
                }
            }
        }

        protected override void PerformAttack()
        {
            GameObject bullet = GetPooledBullet();
            if (bullet == null) return;

            bullet.transform.position = _agent.transform.position + _agent.transform.forward;
            bullet.transform.rotation = Quaternion.LookRotation(_target.position - _agent.transform.position);
            bullet.SetActive(true);
        }

        protected override bool IsOutOfRange() => false;

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

            GameObject bullet = GameObject.Instantiate(bulletPrefab);
            bullet.SetActive(false);
            _bulletPool.Add(bullet);
            return bullet;
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

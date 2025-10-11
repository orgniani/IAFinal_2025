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
        [SerializeField] private EnemyGeneralSettings generalSettings;
        [SerializeField] private RangedEnemySettings settings;
        [SerializeField] private GameObject bulletPrefab;

        [Header("Pooling Settings")]
        [SerializeField] private int minPoolSize = 5;
        [SerializeField] private int maxPoolSize = 30;

        private List<GameObject> _bulletPool;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            if (_bulletPool == null)
                InitializePool(minPoolSize);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            if (_target == null)
                return;

            float distance = Vector3.Distance(_agent.transform.position, _target.position);

            if (!_damageableTarget.IsAlive)
            {
                animator.SetTrigger(animationParameters.playerDiedTrigger);
                return;
            }

            if (distance < settings.escapeDistance)
            {
                animator.SetTrigger(animationParameters.playerTooCloseTrigger);
                return;
            }

            if (distance > settings.loseTargetDistance)
            {
                animator.SetTrigger(animationParameters.playerLostTrigger);
                return;
            }

            if (distance > settings.attackRange && distance <= settings.loseTargetDistance)
            {
                animator.SetTrigger(animationParameters.playerOutRangeTrigger);
                return;
            }

            _cooldownTimer -= Time.deltaTime;
            if (_cooldownTimer <= 0f)
            {
                _cooldownTimer = attackCooldown;
                PerformAttack();
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

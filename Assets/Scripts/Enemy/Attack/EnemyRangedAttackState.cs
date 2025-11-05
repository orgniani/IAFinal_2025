using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Damage;
using Player;
using Helpers;

namespace Enemy
{
    public class EnemyRangedAttackState : EnemyAttackStateBase
    {
        [Header("Ranged Attack Settings")]
        [SerializeField, Range(0f, 20f)] private float escapeRange = 3f;
        [SerializeField] private Bullet bulletPrefab;

        [Header("Pooling Settings")]
        [SerializeField] private int minPoolSize = 5;
        [SerializeField] private int maxPoolSize = 30;

        private List<Bullet> _bullets;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            ReferenceValidator.Validate(bulletPrefab, nameof(bulletPrefab), this);

            if (_bullets == null)
                InitializePool(minPoolSize);

            _agent.isStopped = false;
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

            Vector3 lookPos = _target.position - _agent.transform.position;
            lookPos.y = 0f;
            _agent.transform.rotation = Quaternion.LookRotation(lookPos);

            if (DistanceHelper.IsWithinRange(_agent.transform.position, _target.position, escapeRange))
            {
                animator.SetBool(animationParameters.isPlayerTooCloseBool, true);
                return;
            }

            _agent.ResetPath();
            base.OnStateUpdate(animator, stateInfo, layerIndex);
        }

        protected override void PerformAttack()
        {
            Bullet bullet = GetPooledBullet();
            if (bullet == null) return;

            bullet.transform.position = _agent.transform.position + _agent.transform.forward;
            bullet.transform.rotation = Quaternion.LookRotation(_target.position - _agent.transform.position);
            bullet.gameObject.SetActive(true);

            Vector3 direction = (_target.position - _agent.transform.position).normalized;
            bullet.SetDirection(direction);
        }

        private void InitializePool(int size)
        {
            _bullets = new List<Bullet>();
            for (int i = 0; i < size; i++)
                AddBulletToPool();
        }

        private Bullet AddBulletToPool()
        {
            if (_bullets.Count >= maxPoolSize)
                return null;

            Bullet bullet = Instantiate(bulletPrefab, _agent.transform);
            bullet.gameObject.SetActive(false);

            _bullets.Add(bullet);
            return bullet;
        }

        private Bullet GetPooledBullet()
        {
            foreach (var bullet in _bullets)
            {
                if (!bullet.gameObject.activeInHierarchy)
                    return bullet;
            }
            return AddBulletToPool();
        }
    }
}

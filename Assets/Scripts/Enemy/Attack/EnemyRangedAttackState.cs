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
        [SerializeField] private GameObject bulletPrefab;

        [Header("Pooling Settings")]
        [SerializeField] private int minPoolSize = 5;
        [SerializeField] private int maxPoolSize = 30;

        private List<GameObject> _bulletPool;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            ReferenceValidator.Validate(bulletPrefab, nameof(bulletPrefab), this);

            if (_bulletPool == null)
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
                animator.SetTrigger(animationParameters.playerTooCloseTrigger);
                return;
            }

            _agent.ResetPath();
            base.OnStateUpdate(animator, stateInfo, layerIndex);
        }


        protected override void PerformAttack()
        {
            GameObject bullet = GetPooledBullet();
            if (bullet == null) return;

            bullet.transform.position = _agent.transform.position + _agent.transform.forward;
            bullet.transform.rotation = Quaternion.LookRotation(_target.position - _agent.transform.position);
            bullet.SetActive(true);
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

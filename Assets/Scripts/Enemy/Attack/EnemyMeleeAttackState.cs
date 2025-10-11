using UnityEngine;

namespace Enemy
{
    public class EnemyMeleeAttackState : EnemyAttackStateBase
    {
        [SerializeField] private float attackDamage = 10f;

        protected override void PerformAttack()
        {
            _damageableTarget?.ApplyDamage(attackDamage);
        }

        protected override bool IsOutOfRange()
        {
            float sqrDist = (_target.position - _agent.transform.position).sqrMagnitude;
            return sqrDist > attackRange * attackRange;
        }
    }
}

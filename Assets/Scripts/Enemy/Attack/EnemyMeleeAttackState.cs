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

    }
}

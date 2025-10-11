using UnityEngine;
using UnityEngine.AI;
using Damage;
using Player;

namespace Enemy
{
    public class EnemyAttackState : StateMachineBehaviour
    {
        [Header("Settings")]
        [SerializeField, Range(0f, 5f)] private float attackCooldown = 1f;
        [SerializeField, Range(0f, 5f)] private float attackRange = 2f;
        [SerializeField] private float attackDamage = 10f;

        [Header("Animator Parameters")]
        [SerializeField] private string playerOutOfRangeTrigger = "PlayerOutOfRange";
        [SerializeField] private string playerDiedTrigger = "PlayerDied";

        private NavMeshAgent _agent;

        private Transform _target;
        private IDamageable _damageableTarget;

        private float _cooldownTimer;

        private bool IsOutOfRange()
        {
            float sqrDistance = (_target.position - _agent.transform.position).sqrMagnitude;
            return sqrDistance > attackRange * attackRange;
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _agent = animator.GetComponent<NavMeshAgent>();
            var player = FindAnyObjectByType<PlayerController>();

            if (player)
            {
                _target = player.transform;
                _damageableTarget = player.GetComponent<IDamageable>();
            }

            _agent.isStopped = true;
            _cooldownTimer = 0f;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_target == null || _damageableTarget == null)
                return;

            if (!_damageableTarget.IsAlive)
            {
                animator.SetTrigger(playerDiedTrigger);
                return;
            }

            Vector3 lookPos = _target.position - _agent.transform.position;
            lookPos.y = 0f;
            _agent.transform.rotation = Quaternion.LookRotation(lookPos);

            if (IsOutOfRange())
            {
                animator.SetTrigger(playerOutOfRangeTrigger);
                return;
            }

            _cooldownTimer -= Time.deltaTime;
            if (_cooldownTimer <= 0f)
            {
                _cooldownTimer = attackCooldown;
                _damageableTarget.ApplyDamage(attackDamage);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _agent.isStopped = false;
        }
    }
}

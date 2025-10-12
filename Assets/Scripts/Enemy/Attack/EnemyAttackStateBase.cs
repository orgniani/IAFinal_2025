using UnityEngine;
using UnityEngine.AI;
using Damage;
using Player;

namespace Enemy
{
    public abstract class EnemyAttackStateBase : StateMachineBehaviour
    {
        [Header("Parameters")]
        [SerializeField] protected EnemyAnimationParameters animationParameters;

        [Header("Shared Settings")]
        [SerializeField] protected EnemyGeneralSettings settings;
        [SerializeField, Range(0f, 5f)] protected float attackCooldown = 1f;

        protected NavMeshAgent _agent;
        protected Transform _target;
        protected IDamageable _damageableTarget;
        protected float _cooldownTimer;

        protected abstract void PerformAttack();
        protected abstract bool IsOutOfRange();

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
                animator.SetTrigger(animationParameters.playerDiedTrigger);
                return;
            }

            Vector3 lookPos = _target.position - _agent.transform.position;
            lookPos.y = 0f;
            _agent.transform.rotation = Quaternion.LookRotation(lookPos);

            if (IsOutOfRange())
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

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _agent.isStopped = false;
        }
    }
}

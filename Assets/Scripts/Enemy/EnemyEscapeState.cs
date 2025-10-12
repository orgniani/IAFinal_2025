using UnityEngine;
using UnityEngine.AI;
using Damage;
using Player;

namespace Enemy
{
    public class EnemyEscapeState : StateMachineBehaviour
    {
        [Header("Parameters")]
        [SerializeField] private EnemyAnimationParameters animationParameters;

        [Header("Settings")]
        [SerializeField] private EnemyGeneralSettings settings;
        [SerializeField, Range(0f, 10f)] private float escapeSpeed = 6f;

        private NavMeshAgent _agent;
        private Transform _target;
        private IDamageable _damageableTarget;
        private float _timer;

        private bool IsOutOfAttackRange()
        {
            float dist = Vector3.Distance(_agent.transform.position, _target.position);
            return dist > settings.attackRange;
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

            _agent.isStopped = false;
            _agent.speed = escapeSpeed;
            _timer = 0f;
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

            _timer += Time.deltaTime;
            if (_timer >= 0.3f)
            {
                _timer = 0f;
                Vector3 dirAway = (_agent.transform.position - _target.position).normalized;
                Vector3 destination = _agent.transform.position + dirAway * settings.attackRange;
                _agent.SetDestination(destination);
            }

            if (IsOutOfAttackRange())
                animator.SetTrigger(animationParameters.playerOutRangeTrigger);
            else
                animator.SetTrigger(animationParameters.playerInRangeTrigger);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _agent.ResetPath();
        }
    }
}

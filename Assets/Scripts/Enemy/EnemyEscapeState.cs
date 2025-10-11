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
        [SerializeField] private RangedEnemySettings settings;

        private NavMeshAgent _agent;
        private Transform _target;
        private IDamageable _damageableTarget;
        private float _timer;

        private bool IsFarEnough()
        {
            float dist = Vector3.Distance(_agent.transform.position, _target.position);
            return dist >= settings.attackRange;
        }

        private bool IsOutOfRange()
        {
            float dist = Vector3.Distance(_agent.transform.position, _target.position);
            return dist > settings.loseTargetDistance;
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
            _agent.speed = settings.escapeSpeed;
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

            //TODO: not out of range, it should check if outside ATTACK DISTANCE, no LOST DISTANce!!! change this up!
            if (IsOutOfRange())
                animator.SetTrigger(animationParameters.playerLostTrigger);
            else if (IsFarEnough())
                animator.SetTrigger(animationParameters.playerInRangeTrigger);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _agent.ResetPath();
        }
    }
}

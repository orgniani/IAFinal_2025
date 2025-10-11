using UnityEngine;
using UnityEngine.AI;
using Damage;
using Player;

namespace Enemy
{
    public class EnemyChaseState : StateMachineBehaviour
    {
        [Header("Settings")]
        [SerializeField, Range(0f, 20f)] private float chaseSpeed = 5f;
        [SerializeField, Range(0f, 30f)] private float loseRange = 15f;
        [SerializeField, Range(0f, 2f)] private float updateInterval = 0.3f;
        [SerializeField, Range(0f, 5f)] private float attackRange = 2f;

        [Header("Animator Parameters")]
        [SerializeField] private string playerLostTrigger = "PlayerOutOfRange";
        [SerializeField] private string playerInAttackRangeTrigger = "PlayerInAttackRange";
        [SerializeField] private string playerDiedTrigger = "PlayerDied";

        private NavMeshAgent _agent;

        private Transform _target;
        private IDamageable _damageableTarget;

        private float _timer;

        private bool IsOutOfRange() =>
            (_target.position - _agent.transform.position).sqrMagnitude > loseRange * loseRange;

        private bool IsInAttackRange() =>
            (_target.position - _agent.transform.position).sqrMagnitude <= attackRange * attackRange;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _agent = animator.GetComponent<NavMeshAgent>();
            var player = FindAnyObjectByType<PlayerController>();

            if (player)
            {
                _target = player.transform;
                _damageableTarget = player.GetComponent<IDamageable>();
            }

            _agent.speed = chaseSpeed;
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

            _timer += Time.deltaTime;
            if (_timer >= updateInterval)
            {
                _agent.SetDestination(_target.position);
                _timer = 0f;
            }

            if (IsOutOfRange())
                animator.SetTrigger(playerLostTrigger);
            else if (IsInAttackRange())
                animator.SetTrigger(playerInAttackRangeTrigger);
        }
    }
}

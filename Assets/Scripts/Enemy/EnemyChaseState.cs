using UnityEngine;
using UnityEngine.AI;
using Damage;
using Player;
using Helpers;

namespace Enemy
{
    public class EnemyChaseState : StateMachineBehaviour
    {
        [Header("Settings")]
        [SerializeField] private EnemyGeneralSettings settings;
        [SerializeField, Range(0f, 20f)] private float chaseSpeed = 5f;
        [SerializeField, Range(0f, 30f)] private float loseRange = 15f;

        [Header("Parameters")]
        [SerializeField] private EnemyAnimationParameters animationParameters;

        private NavMeshAgent _agent;
        private Transform _target;
        private IDamageable _damageableTarget;
        private float _timer;

        private bool IsOutOfRange()
        {
            return DistanceHelper.IsBeyondRange(_agent.transform.position, _target.position, loseRange);
        }

        private bool IsInAttackRange()
        {
            return DistanceHelper.IsWithinRange(_agent.transform.position, _target.position, settings.AttackRange);
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ValidateReferences();

            _agent = animator.GetComponent<NavMeshAgent>();
            var player = FindAnyObjectByType<PlayerController>();
            if (player)
            {
                _target = player.transform;
                _damageableTarget = player.GetComponent<IDamageable>();
            }

            _agent.speed = chaseSpeed;
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
            if (_timer >= settings.UpdateInterval)
            {
                _agent.SetDestination(_target.position);
                _timer = 0f;
            }

            if (IsInAttackRange())
                animator.SetBool(animationParameters.isPlayerInRangeBool, true);
            else if (IsOutOfRange())
                animator.SetBool(animationParameters.isPlayerDetectedBool, false);
        }

        private void ValidateReferences()
        {
            ReferenceValidator.Validate(animationParameters, nameof(animationParameters), this);
            ReferenceValidator.Validate(settings, nameof(settings), this);
        }
    }
}

using UnityEngine;
using UnityEngine.AI;
using Damage;
using Player;
using Helpers;
using Speed;

namespace Enemy
{
    public class EnemyChaseState : StateMachineBehaviour
    {
        [Header("Settings")]
        [SerializeField] private EnemyGeneralSettings settings;
        [SerializeField, Range(0f, 20f)] private float chaseSpeed = 5f;
        [SerializeField, Range(0f, 30f)] private float loseRange = 15f;
        [SerializeField] private bool canJump = false;

        [Header("Parameters")]
        [SerializeField] private EnemyAnimationParameters animationParameters;

        private NavMeshAgent _agent;
        private SpeedModifier _speedMod;
        private Transform _target;

        private IDamageable _damageableTarget;
        private IDamageable _selfDamageable;

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
            _speedMod = animator.GetComponent<SpeedModifier>();
            _selfDamageable = animator.GetComponent<IDamageable>();

            var player = FindAnyObjectByType<PlayerController>();
            if (player)
            {
                _target = player.transform;
                _damageableTarget = player.GetComponent<IDamageable>();
            }

            _speedMod.SetSpeed(chaseSpeed);
            _timer = 0f;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_target == null || _damageableTarget == null)
            {
                Debug.LogWarning("[EnemyChaseState] Target or DamageableTarget is null, aborting update.");
                return;
            }

            if (!_damageableTarget.IsAlive)
            {
                animator.SetTrigger(animationParameters.playerDiedTrigger);
                return;
            }

            if (_agent.isOnOffMeshLink && canJump)
            {
                animator.SetBool(animationParameters.isJumpingBool, true);
                return;
            }

            _timer += Time.deltaTime;
            if (_timer >= settings.UpdateInterval)
            {
                _agent.SetDestination(_target.position);
                _timer = 0f;
            }

            if (IsInAttackRange())
            {
                if (_selfDamageable != null)
                    _selfDamageable.ClearAggroLock();

                animator.SetBool(animationParameters.isPlayerInRangeBool, true);
                return;
            }

            if (_selfDamageable != null && _selfDamageable.AggroLocked) return;

            if (IsOutOfRange())
                animator.SetBool(animationParameters.isPlayerDetectedBool, false);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            bool playerDetected = animator.GetBool(animationParameters.isPlayerDetectedBool);
            bool playerInRange = animator.GetBool(animationParameters.isPlayerInRangeBool);
            bool isJumping = animator.GetBool(animationParameters.isJumpingBool);

            Debug.Log($"[EnemyChaseState] EXIT triggered! " +
                      $"AggroLock={_selfDamageable?.AggroLocked}, " +
                      $"Detected={playerDetected}, InRange={playerInRange}, Jumping={isJumping}");
        }

        private void ValidateReferences()
        {
            ReferenceValidator.Validate(animationParameters, nameof(animationParameters), this);
            ReferenceValidator.Validate(settings, nameof(settings), this);
        }
    }
}

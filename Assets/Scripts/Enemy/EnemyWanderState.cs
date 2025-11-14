using UnityEngine;
using UnityEngine.AI;
using Player;
using Damage;
using Helpers;
using Speed;

namespace Enemy
{
    public class EnemyWanderState : StateMachineBehaviour
    {
        [Header("Parameters")]
        [SerializeField] private EnemyAnimationParameters animationParameters;

        [Header("Settings")]
        [SerializeField] private EnemyGeneralSettings settings;
        [SerializeField, Range(0f, 20f)] private float wanderSpeed = 3f;
        [SerializeField, Range(0f, 30f)] private float searchRadius = 10f;
        [SerializeField, Range(0f, 20f)] private float circleRadius = 3f;
        [SerializeField, Range(4, 64)] private int circlePrecision = 16;

        private NavMeshAgent _agent;
        private SpeedModifier _speedMod;

        private Transform _target;
        private IDamageable _damageableTarget;

        private HealthController _selfHealth;

        private Vector3 _origin;
        private float _angle;
        private bool _forceAgroFromHit = false;

        private Vector3 GetNextDestination()
        {
            float radians = _angle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(radians), 0f, Mathf.Sin(radians)) * circleRadius;
            _angle = (_angle + 360f / circlePrecision) % 360f;
            return _origin + offset;
        }

        private bool IsPlayerInRange()
        {
            if (_target == null)
                return false;

            return DistanceHelper.IsWithinRange(_agent.transform.position, _target.position, searchRadius);
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ValidateReferences();

            _agent = animator.GetComponent<NavMeshAgent>();
            _speedMod = animator.GetComponent<SpeedModifier>();
            _selfHealth = animator.GetComponent<HealthController>();

            if (_selfHealth != null)
                _selfHealth.OnHit += HandleHit;

            var player = FindAnyObjectByType<PlayerController>();
            if (player)
            {
                _target = player.transform;
                _damageableTarget = player.GetComponent<IDamageable>();
            }

            _origin = _agent.transform.position;
            _speedMod.SetSpeed(wanderSpeed);
            _agent.SetDestination(GetNextDestination());

            _forceAgroFromHit = false;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_agent == null)
                return;

            if (_agent.remainingDistance <= _agent.stoppingDistance)
                _agent.SetDestination(GetNextDestination());

            if (_damageableTarget == null || !_damageableTarget.IsAlive || _target == null)
            {
                animator.SetBool(animationParameters.isPlayerDetectedBool, false);
                return;
            }

            if (_forceAgroFromHit)
            {
                animator.SetBool(animationParameters.isPlayerDetectedBool, true);
                return;
            }

            animator.SetBool(animationParameters.isPlayerDetectedBool, IsPlayerInRange());
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_selfHealth != null)
                _selfHealth.OnHit -= HandleHit;
        }

        private void HandleHit()
        {
            _forceAgroFromHit = true;
        }

        private void ValidateReferences()
        {
            ReferenceValidator.Validate(animationParameters, nameof(animationParameters), this);
            ReferenceValidator.Validate(settings, nameof(settings), this);
        }
    }
}

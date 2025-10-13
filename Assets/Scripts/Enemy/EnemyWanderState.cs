using UnityEngine;
using UnityEngine.AI;
using Player;
using Damage;
using Helpers;

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

        private Transform _target;
        private IDamageable _damageableTarget;

        private Vector3 _origin;
        private float _angle;

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

            var player = FindAnyObjectByType<PlayerController>();
            if (player)
            {
                _target = player.transform;
                _damageableTarget = player.GetComponent<IDamageable>();
            }

            _origin = _agent.transform.position;
            _agent.speed = wanderSpeed;
            _agent.SetDestination(GetNextDestination());
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_agent.remainingDistance <= _agent.stoppingDistance)
                _agent.SetDestination(GetNextDestination());

            if (_damageableTarget != null && _damageableTarget.IsAlive && IsPlayerInRange())
                animator.SetBool(animationParameters.isPlayerDetectedBool, true);
        }

        private void ValidateReferences()
        {
            ReferenceValidator.Validate(animationParameters, nameof(animationParameters), this);
            ReferenceValidator.Validate(settings, nameof(settings), this);
        }
    }
}

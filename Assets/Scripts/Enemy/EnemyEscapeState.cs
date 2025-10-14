using UnityEngine;
using UnityEngine.AI;
using Damage;
using Player;
using Helpers;

namespace Enemy
{
    public class EnemyEscapeState : StateMachineBehaviour
    {
        [Header("Parameters")]
        [SerializeField] private EnemyAnimationParameters animationParameters;

        [Header("Settings")]
        [SerializeField] private EnemyGeneralSettings settings;
        [SerializeField, Range(0f, 15f)] private float optimalDistance = 8f;
        [SerializeField, Range(0f, 10f)] private float escapeSpeed = 6f;

        private NavMeshAgent _agent;
        private Transform _target;
        private IDamageable _damageableTarget;
        private float _timer;
        private RangeState _rangeState;

        private enum RangeState { TooClose, InOptimal, TooFar }

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

            _agent.isStopped = false;
            _agent.speed = escapeSpeed;

            _timer = 0f;
            _rangeState = RangeState.InOptimal;
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
            if (_timer < settings.UpdateInterval)
                return;
            _timer = 0f;

            float sqrDistance = DistanceHelper.GetSqrDistance(_agent.transform.position, _target.position);

            MoveAccordingToDistance(sqrDistance);
            UpdateRangeState(animator, sqrDistance);
        }

        private void MoveAccordingToDistance(float sqrDistance)
        {
            float optimalSqr = optimalDistance * optimalDistance;

            if (sqrDistance >= optimalSqr)
            {
                _agent.ResetPath();
                return;
            }

            Vector3 enemyPos = _agent.transform.position;
            Vector3 playerPos = _target.position;
            Vector3 dir = (enemyPos - playerPos).normalized;

            float currentDistance = DistanceHelper.GetDistance(_agent.transform.position, _target.position);
            float retreatAmount = optimalDistance - currentDistance + 1f;

            _agent.SetDestination(enemyPos + dir * retreatAmount);
        }

        private void UpdateRangeState(Animator animator, float sqrDistance)
        {
            float optimalDistanceSqr = optimalDistance * optimalDistance;

            RangeState newState =
                sqrDistance < optimalDistanceSqr ? RangeState.TooClose :
                sqrDistance > optimalDistanceSqr ? RangeState.TooFar :
                RangeState.InOptimal;

            if (newState == _rangeState)
                return;

            _rangeState = newState;

            switch (newState)
            {
                case RangeState.InOptimal:
                    animator.SetBool(animationParameters.isPlayerTooCloseBool, false);
                    break;
                case RangeState.TooFar:
                    animator.SetBool(animationParameters.isPlayerTooCloseBool, false);
                    break;
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _agent.ResetPath();
        }

        private void ValidateReferences()
        {
            ReferenceValidator.Validate(animationParameters, nameof(animationParameters), this);
            ReferenceValidator.Validate(settings, nameof(settings), this);
        }
    }
}

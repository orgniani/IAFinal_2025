using UnityEngine;
using UnityEngine.AI;
using Player;
using Damage;

namespace Enemy
{
    public class EnemyWanderState : StateMachineBehaviour
    {
        [Header("Settings")]
        [SerializeField, Range(0f, 20f)] private float wanderSpeed = 3f;
        [SerializeField, Range(0f, 30f)] private float searchRadius = 10f;
        [SerializeField, Range(0f, 20f)] private float circleRadius = 3f;
        [SerializeField, Range(4, 64)] private int circlePrecision = 16;

        [Header("Animator Parameters")]
        [SerializeField] private string playerDetectedTrigger = "PlayerDetected";

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

            float sqrDistance = (_target.position - _agent.transform.position).sqrMagnitude;
            return sqrDistance <= searchRadius * searchRadius;
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

            _origin = _agent.transform.position;
            _agent.speed = wanderSpeed;
            _agent.SetDestination(GetNextDestination());
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_agent.remainingDistance <= _agent.stoppingDistance)
                _agent.SetDestination(GetNextDestination());

            if (_damageableTarget != null && _damageableTarget.IsAlive && IsPlayerInRange())
                animator.SetTrigger(playerDetectedTrigger);
        }
    }
}

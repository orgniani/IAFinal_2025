using Damage;
using UnityEngine;
using UnityEngine.AI;
using System;
using Speed;

namespace Player
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(HealthController))]
    [RequireComponent(typeof(SpeedModifier))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Data Source")]
        [SerializeField] private HealthControllerSource healthControllerSource;

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float rotationSpeed = 720f;

        private NavMeshAgent _agent;
        private HealthController _health;
        private SpeedModifier _speedMod;

        public event Action<bool, Transform> OnShoot;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _health = GetComponent<HealthController>();
            _speedMod = GetComponent<SpeedModifier>();
        }

        private void OnEnable()
        {
            if (!healthControllerSource.DataInstance)
                healthControllerSource.DataInstance = _health;
        }

        private void OnDisable()
        {
            if (healthControllerSource.DataInstance == _health)
                healthControllerSource.DataInstance = null;
        }

        private void Start()
        {
            _speedMod.SetSpeed(moveSpeed);
            _agent.updateRotation = false;
        }

        public void MoveToClickPoint(Vector3 worldPoint)
        {
            if (!_health.IsAlive)
                return;

            if (!_agent.isOnNavMesh)
            {
                Debug.LogWarning($"[PlayerController] Agent is not on NavMesh at {transform.position}");
                return;
            }

            _agent.SetDestination(worldPoint);
        }

        public void Shoot(bool shooting, Transform target)
        {
            if (!_health.IsAlive)
                return;

            OnShoot?.Invoke(shooting, target);
        }

        public void RotateTowards(Vector3 worldPos)
        {
            if (!_health.IsAlive)
                return;

            Vector3 direction = worldPos - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.001f)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}

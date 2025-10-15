using Damage;
using UnityEngine;
using UnityEngine.AI;
using System;

namespace Player
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Health))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float rotationSpeed = 720f;

        private NavMeshAgent _agent;
        private Health _health;

        public event Action<bool> OnShoot;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _health = GetComponent<Health>();

            _agent.speed = moveSpeed;
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


        public void Shoot(bool shooting)
        {
            if (!_health.IsAlive)
                return;

            OnShoot?.Invoke(shooting);
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

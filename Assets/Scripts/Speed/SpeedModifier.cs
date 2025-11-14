using UnityEngine;
using UnityEngine.AI;

namespace Speed
{
    public class SpeedModifier : MonoBehaviour
    {
        private NavMeshAgent _agent;
        private float _currentSpeed = 0f;
        private float _currentMultiplier = 1f;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _currentSpeed = _agent.speed;
        }

        public void ApplyMultiplier(float multiplier)
        {
            _currentMultiplier = multiplier;
            RecalculateSpeed();
        }

        public void ClearMultiplier()
        {
            _currentMultiplier = 1f;
            RecalculateSpeed();
        }

        private void RecalculateSpeed()
        {
            if (!_agent) return;
            _agent.speed = _currentSpeed * _currentMultiplier;

            Debug.Log($"[SpeedModifier]{gameObject.name} | Base speed set to {_currentSpeed}, final speed = {_agent.speed}");
        }

        public void SetSpeed(float newSpeed)
        {
            _currentSpeed = newSpeed;
            RecalculateSpeed();
        }
    }
}

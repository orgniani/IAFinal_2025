using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Zones
{
    public class MudZone : MonoBehaviour
    {
        [SerializeField] private float speedReductionPerc = 0.5f;

        private readonly Dictionary<NavMeshAgent, float> _originalSpeeds = new();

        private void OnTriggerEnter(Collider other)
        {
            var agent = other.GetComponent<NavMeshAgent>();
            if (!agent || _originalSpeeds.ContainsKey(agent)) return;

            _originalSpeeds[agent] = agent.speed;
            agent.speed *= speedReductionPerc;
            Debug.Log($"[MudZone] Reduced speed of {other.name} to {agent.speed}");
        }

        private void OnTriggerExit(Collider other)
        {
            var agent = other.GetComponent<NavMeshAgent>();
            if (!agent || !_originalSpeeds.ContainsKey(agent)) return;

            agent.speed = _originalSpeeds[agent];
            _originalSpeeds.Remove(agent);
            Debug.Log($"[MudZone] Restored speed of {other.name} to {agent.speed}");
        }
    }
}
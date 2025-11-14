using UnityEngine;
using UnityEngine.AI;
using Speed;

namespace Zones
{
    public class SlowZone : MonoBehaviour
    {
        [SerializeField] private float speedReductionPerc = 0.5f;

        private void OnTriggerEnter(Collider other)
        {
            var mod = other.GetComponent<SpeedModifier>();
            if (!mod) return;

            mod.ApplyMultiplier(speedReductionPerc);

            var agent = other.GetComponent<NavMeshAgent>();
            if (agent)
                Debug.Log($"[SlowZone] Reduced speed of {other.name} to {agent.speed}");
        }

        private void OnTriggerExit(Collider other)
        {
            var mod = other.GetComponent<SpeedModifier>();
            if (!mod) return;

            mod.ClearMultiplier();

            var agent = other.GetComponent<NavMeshAgent>();
            if (agent)
                Debug.Log($"[SlowZone] Restored speed of {other.name} to {agent.speed}");
        }
    }
}
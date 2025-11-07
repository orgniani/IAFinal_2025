using UnityEngine;
using UnityEngine.AI;

namespace Zones
{
    public class MudZone : MonoBehaviour
    {
        [SerializeField] private float speedReductionPerc = 0.5f;

        void OnTriggerEnter (Collider other)
        {
            NavMeshAgent agent = other.gameObject.GetComponentInParent<NavMeshAgent>();

            if (agent)
                agent.speed *= speedReductionPerc;
        }
        
        void OnTriggerExit (Collider other)
        {
            NavMeshAgent agent = other.gameObject.GetComponentInParent<NavMeshAgent>();

            if (agent)
                agent.speed /= speedReductionPerc;
        }
    }
}
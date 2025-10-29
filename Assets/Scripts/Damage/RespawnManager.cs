using System.Collections;
using UnityEngine;

namespace Damage
{
    public class RespawnManager : MonoBehaviour
    {
        public static RespawnManager Instance;

        private void Awake() => Instance = this;

        public void ScheduleRespawn(HealthController target, float cooldown)
        {
            StartCoroutine(RespawnRoutine(target, cooldown));
        }

        private IEnumerator RespawnRoutine(HealthController target, float cooldown)
        {
            yield return new WaitForSeconds(cooldown);
            target.Respawn();
        }
    }
}

using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "EnemyGeneralSettings", menuName = "Enemy/General Settings")]
    public class EnemyGeneralSettings : ScriptableObject
    {
        [Header("General")]
        public float chaseSpeed = 5f;

        [Tooltip("If the player is beyond this, the enemy loses sight and returns to wandering.")]
        public float loseTargetDistance = 15f;
        public float updateInterval = 0.3f;

        [Tooltip("If the player is within this range, the enemy attacks.")]
        public float attackRange = 8f;
    }   

}
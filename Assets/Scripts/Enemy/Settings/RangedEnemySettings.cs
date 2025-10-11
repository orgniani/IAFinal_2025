using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "RangedEnemySettings", menuName = "Enemy/Ranged Settings")]
    public class RangedEnemySettings : EnemyGeneralSettings
    {
        [Header("Distances")]
        [Tooltip("If the player is closer than this, the enemy retreats to maintain distance.")]
        public float escapeDistance = 3f;

        [Header("Speeds")]
        public float escapeSpeed = 6f;
    }
}
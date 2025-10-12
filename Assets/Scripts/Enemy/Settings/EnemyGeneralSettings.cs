using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "EnemyGeneralSettings", menuName = "Enemy/General Settings")]
    public class EnemyGeneralSettings : ScriptableObject
    {
        [Tooltip("If the player is beyond this, the enemy loses sight and returns to wandering.")]
        [Range(0f, 2f)] public float updateInterval = 0.3f;

        [Tooltip("If the player is within this range, the enemy attacks.")]
        [Range(0f, 10f)] public float attackRange = 2f;
    }   

}
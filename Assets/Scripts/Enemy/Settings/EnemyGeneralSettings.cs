using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "EnemyGeneralSettings", menuName = "Enemy/General Settings")]
    public class EnemyGeneralSettings : ScriptableObject
    {
        [Tooltip("Time (in seconds) between NavMesh path recalculations during chase or patrol. "
               + "Higher values reduce CPU load but make enemies less responsive.")]
        [Range(0f, 2f)] public float UpdateInterval = 0.3f;

        [Tooltip("If the player is within this range, the enemy attacks.")]
        [Range(0f, 10f)] public float AttackRange = 2f;
    }
}

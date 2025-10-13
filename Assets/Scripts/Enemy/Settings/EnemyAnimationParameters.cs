using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "EnemyAnimationParameters", menuName = "Enemy/Enemy Animation Parameters")]
    public class EnemyAnimationParameters : ScriptableObject
    {
        [Header("Parameters")]
        public string isPlayerDetectedBool = "PlayerDetected";
        public string isPlayerInRangeBool = "PlayerInRange";
        public string isPlayerTooCloseBool = "PlayerTooClose";
        public string playerDiedTrigger = "PlayerDied";
    }

}
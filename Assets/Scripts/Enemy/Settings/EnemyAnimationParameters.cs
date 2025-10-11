using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "EnemyAnimationParameters", menuName = "Enemy/EnemyAnimationParameters")]
    public class EnemyAnimationParameters : ScriptableObject
    {
        [Header("Parameters")]
        public string playerDetectedTrigger = "PlayerDetected";
        public string playerInRangeTrigger = "PlayerInAttackRange";
        public string playerOutRangeTrigger = "PlayerOutAttackRange";
        public string playerLostTrigger = "PlayerLost";
        public string playerDiedTrigger = "PlayerDied";
        public string playerTooCloseTrigger = "PlayerTooClose";
    }

}
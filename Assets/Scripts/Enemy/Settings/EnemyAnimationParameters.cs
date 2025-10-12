using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "EnemyAnimationParameters", menuName = "Enemy/Enemy Animation Parameters")]
    public class EnemyAnimationParameters : ScriptableObject
    {
        [Header("Parameters")]
        public string playerDetectedTrigger = "PlayerDetected";
        public string playerLostTrigger = "PlayerLost";
        public string playerDiedTrigger = "PlayerDied";
        public string playerInRangeTrigger = "PlayerInAttackRange";
        public string playerOutRangeTrigger = "PlayerOutAttackRange";
        public string playerTooCloseTrigger = "PlayerTooClose";
    }

}
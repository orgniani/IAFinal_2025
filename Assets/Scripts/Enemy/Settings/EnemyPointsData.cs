using System;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "EnemyPointsData", menuName = "Data/Enemy Points Data")]
    public class EnemyPointsData : ScriptableObject
    {
        [Serializable]
        public class EnemyPointEntry
        {
            public EnemyType enemyType;
            public int points;
        }

        public List<EnemyPointEntry> entries = new();

        public int GetPointsForType(EnemyType type)
        {
            foreach (var e in entries)

                if (e.enemyType == type)
                    return e.points;

            return 0;
        }
    }
}

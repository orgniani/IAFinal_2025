using System;
using UnityEngine;
using Enemy;
using DataSource;

namespace Managers
{
    public class PointsManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private EnemyPointsData pointsData;
        [SerializeField] private int currentPoints = 0;

        public event Action<int> OnPointsChanged = delegate { };

        private void OnEnable()
        {
            SubscribeToEnemies();
            ResetPoints();
        }

        private void OnDisable()
        {
            UnsubscribeFromEnemies();
        }

        private void SubscribeToEnemies()
        {
            var enemies = FindObjectsByType<EnemyIdentifier>(FindObjectsSortMode.None);

            foreach (var enemy in enemies)
                enemy.OnDeath += HandleEnemyDeath;
        }

        private void UnsubscribeFromEnemies()
        {
            var enemies = FindObjectsByType<EnemyIdentifier>(FindObjectsSortMode.None);

            foreach (var enemy in enemies)
                enemy.OnDeath -= HandleEnemyDeath;
        }

        private void HandleEnemyDeath(EnemyType type)
        {
            int gained = pointsData.GetPointsForType(type);
            currentPoints += gained;
            OnPointsChanged.Invoke(currentPoints);

            Debug.Log($"[PointsManager] +{gained} from {type}. Total: {currentPoints}");
        }

        public void ResetPoints()
        {
            currentPoints = 0;
            OnPointsChanged.Invoke(currentPoints);
        }
    }
}

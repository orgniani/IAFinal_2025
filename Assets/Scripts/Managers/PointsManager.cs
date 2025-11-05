using System;
using UnityEngine;
using Enemy;
using DataSource;
using Events;

namespace Managers
{
    public class PointsManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private EnemyPointsData pointsData;

        [Header("State")]
        [SerializeField] private int currentPoints = 0;
        private int _highScore;

        private const string HIGH_SCORE_KEY = "HighScore";

        public event Action<int> OnPointsChanged = delegate { };

        private void OnEnable()
        {
            SubscribeToEnemies();
            LoadHighScore();
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

            if (currentPoints > _highScore)
                UpdateHighScore(currentPoints);

            Debug.Log($"[PointsManager] +{gained} from {type}. Total: {currentPoints}");
        }

        private void UpdateHighScore(int newScore)
        {
            _highScore = newScore;
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, _highScore);
            PlayerPrefs.Save();

            Debug.Log($"[PointsManager] New high score saved: {_highScore}");
        }

        public void ResetPoints()
        {
            currentPoints = 0;
            OnPointsChanged.Invoke(currentPoints);
        }

        private void LoadHighScore()
        {
            _highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
            Debug.Log($"[PointsManager] Loaded high score: {_highScore}");
        }
    }
}

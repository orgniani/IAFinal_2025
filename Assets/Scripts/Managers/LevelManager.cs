using UnityEngine;
using Damage;
using Helpers;
using Events;

namespace Managers
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private HealthController playerHP;
        [SerializeField] private EmptyAction onLoseEvent;

        private void Awake()
        {
            ReferenceValidator.Validate(playerHP, nameof(playerHP), this);
            ReferenceValidator.Validate(onLoseEvent, nameof(onLoseEvent), this);
        }

        private void OnEnable()
        {
            playerHP.OnDie += HandleGameOver;
        }

        private void OnDisable()
        {
            playerHP.OnDie -= HandleGameOver;
        }

        private void HandleGameOver()
        {
            Debug.Log("lose event!");
            onLoseEvent?.InvokeEvent();
        }
    }
}
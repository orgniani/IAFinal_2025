using UnityEngine;
using UnityEngine.UI;
using Damage;
using Helpers;

namespace UI
{
    public class UIHealthBar : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Slider healthBar;
        [SerializeField] private HealthController healthController;

        private void Awake()
        {
            ReferenceValidator.Validate(healthBar, nameof(healthBar), this);
            ReferenceValidator.Validate(healthController, nameof(healthController), this);
        }

        private void OnEnable()
        {
            healthController.OnHit += HandleHealthBar;
        }

        private void Start()
        {
            HandleHealthBar();
        }

        private void OnDisable()
        {
            healthController.OnHit -= HandleHealthBar;
        }

        private void HandleHealthBar()
        {
            if (!healthBar) return;
            healthBar.value = 1.0f * healthController.Health / healthController.MaxHealth;
        }
    }
}
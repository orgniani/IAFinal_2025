using UnityEngine;
using UnityEngine.UI;
using Damage;
using Helpers;
using System.Collections;

namespace UI
{
    public class UIHealthBar : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private HealthController directHealthController;
        [SerializeField] private HealthControllerSource healthControllerSource;
        [SerializeField] private Slider healthBar;

        private HealthController _healthController;
        private Coroutine _initRoutine;

        private void Awake()
        {
            ReferenceValidator.Validate(healthBar, nameof(healthBar), this);
        }

        private void OnEnable()
        {
            _initRoutine = StartCoroutine(InitializeHealthReference());
        }

        private void OnDisable()
        {
            UnsubscribeEvents();

            if (_initRoutine != null)
                StopCoroutine(_initRoutine);
        }

        private IEnumerator InitializeHealthReference()
        {
            yield return null;

            if (directHealthController != null)
            {
                SetHealthController(directHealthController);
                yield break;
            }

            if (healthControllerSource != null)
            {
                while (healthControllerSource.DataInstance == null)
                    yield return null;

                SetHealthController(healthControllerSource.DataInstance);
            }
        }

        private void SetHealthController(HealthController controller)
        {
            UnsubscribeEvents();
            _healthController = controller;

            if (_healthController == null)
            {
                ResetBar();
                return;
            }

            _healthController.OnHit += HandleHealthBar;
            HandleHealthBar();
        }

        private void UnsubscribeEvents()
        {
            if (_healthController != null)
                _healthController.OnHit -= HandleHealthBar;
        }

        private void HandleHealthBar()
        {
            if (!healthBar || _healthController == null)
                return;

            healthBar.value = (float)_healthController.Health / _healthController.MaxHealth;
        }
        private void ResetBar()
        {
            if (healthBar)
                healthBar.value = 1f;
        }
    }
}

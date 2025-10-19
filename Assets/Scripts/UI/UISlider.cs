using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Handles a single volume slider, mute button, and icon toggling for modular volume control.
    /// </summary>
    public class UISlider : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Slider slider;
        [SerializeField] private Button muteButton;

        [Header("Icons")]
        [SerializeField] private Sprite iconOn;
        [SerializeField] private Sprite iconOff;

        private string _volumePrefKey;
        private Action<float> _onVolumeChanged;
        private Action _onButtonClicked;

        private Image _muteButtonImage;
        private float _previousValue = 1f;

        /// <summary>
        /// Initializes the UISlider with a volume key and a volume update callback.
        /// </summary>
        public void Initialize(string volumePrefKey, Action<float> onVolumeChanged, Action onButtonClick)
        {
            _volumePrefKey = volumePrefKey;
            _onVolumeChanged = onVolumeChanged;
            _onButtonClicked = onButtonClick;

            float savedVolume = PlayerPrefs.GetFloat(_volumePrefKey, 1f);
            slider.value = savedVolume;
            _onVolumeChanged?.Invoke(savedVolume);

            slider.onValueChanged.AddListener(HandleSliderValueChanged);
            muteButton.onClick.AddListener(ToggleMute);

            _muteButtonImage = muteButton.GetComponent<Image>();

            UpdateIcon();
        }

        private void HandleSliderValueChanged(float value)
        {
            _onVolumeChanged?.Invoke(value);
            PlayerPrefs.SetFloat(_volumePrefKey, value);
            UpdateIcon();
        }

        private void ToggleMute()
        {
            _onButtonClicked?.Invoke();

            if (slider.value > 0)
            {
                _previousValue = slider.value;
                slider.value = 0;
            }

            else
                slider.value = _previousValue;

            UpdateIcon();
        }

        private void UpdateIcon()
        {
            _muteButtonImage.sprite = slider.value > 0 ? iconOn : iconOff;
        }
    }
}
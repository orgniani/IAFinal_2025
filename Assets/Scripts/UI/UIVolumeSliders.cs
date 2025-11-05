using Audio;
using DataSource;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Acts as the bridge between the AudioManager and the individual UISlider controls.
    /// Initializes the sliders with the correct PlayerPrefs keys, handles audio logic,
    /// and ensures audio feedback only triggers on mute button clicks.
    /// </summary>
    public class UIVolumeSliders : MonoBehaviour
    {
        [Header("Data Sources")]
        [SerializeField] private DataSource<AudioManager> audioManagerDataSource;

        [Header("InvokeEvents")]
        [SerializeField] private AudioEvent onPlayAudio;

        [Header("Sliders")]
        [SerializeField] private UISlider masterSlider;
        [SerializeField] private UISlider musicSlider;
        [SerializeField] private UISlider sfxSlider;

        private string _masterVolume;
        private string _musicVolume;
        private string _sfxVolume;

        private AudioManager _audioManager;

        private void OnEnable()
        {
            if (audioManagerDataSource.DataInstance)
                _audioManager = audioManagerDataSource.DataInstance;

            _masterVolume = _audioManager.MasteVolume;
            _musicVolume = _audioManager.MusicVolume;
            _sfxVolume = _audioManager.SFXVolume;
        }

        private void Start()
        {
            masterSlider.Initialize(_masterVolume, volume => _audioManager.SetMasterVolume(volume), PlayAudio);
            musicSlider.Initialize(_musicVolume, volume => _audioManager.SetMusicVolume(volume), PlayAudio);
            sfxSlider.Initialize(_sfxVolume, volume => _audioManager.SetSFXVolume(volume), PlayAudio);
        }

        private void PlayAudio()
        {
            onPlayAudio?.InvokeEvent(AudioKey.Button_Click);
        }
    }
}

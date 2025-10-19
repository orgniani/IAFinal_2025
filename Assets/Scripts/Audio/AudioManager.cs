using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Audio;
using DataSource;
using Helpers;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        [Header("Data Sources")]
        [SerializeField] private DataSource<AudioManager> audioManagerDataSource;

        [Header("Library")]
        [SerializeField] private AudioLibrary audioLibrary;

        [Header("Subscribe To Event")]
        [SerializeField] private AudioEvent onPlayAudio;

        [Header("Audio Mixer")]
        [SerializeField] private AudioMixer mainMixer;
        [SerializeField] private AudioMixerGroup musicGroup;
        [SerializeField] private AudioMixerGroup sfxGroup;

        [Header("Audio Names")]
        [SerializeField] private string masterVolume = "MasterVolume";
        [SerializeField] private string musicVolume = "MusicVolume";
        [SerializeField] private string sfxVolume = "SFXVolume";

        private readonly List<AudioSource> _audioSourcePool = new();

        public string MasteVolume => masterVolume;
        public string MusicVolume => musicVolume;
        public string SFXVolume => sfxVolume;

        private void Awake()
        {
            ValidateReferences();
        }

        private void OnEnable()
        {
            audioManagerDataSource.DataInstance = this;

            float masterVol = PlayerPrefs.GetFloat(masterVolume, 1);
            float musicVol = PlayerPrefs.GetFloat(musicVolume, 1);
            float sfxVol = PlayerPrefs.GetFloat(sfxVolume, 1);

            SetMasterVolume(masterVol);
            SetMusicVolume(musicVol);
            SetSFXVolume(sfxVol);

            PlayAudioByKey(AudioKey.Main_Music);
            onPlayAudio?.Subscribe(PlayAudioByKey);
        }

        private void OnDisable()
        {
            if (audioManagerDataSource != null && audioManagerDataSource.DataInstance == this)
                audioManagerDataSource.DataInstance = null;

            onPlayAudio?.Unsubscribe(PlayAudioByKey);
        }

        /// <summary>
        /// Plays an audio clip based on the provided AudioKey using a pooled AudioSource.
        /// </summary>
        private void PlayAudioByKey(AudioKey key)
        {
            var config = audioLibrary.GetConfig(key);
            if (config != null)
                PlayAudio(config);
        }

        private void PlayAudio(AudioConfig audioConfig)
        {
            if (audioConfig == null || audioConfig.Clip == null) return;

            AudioSource source = GetPooledAudioSource(audioConfig);
            source.Play();

            if (!audioConfig.Loop)
                StartCoroutine(HandleSourceReturn(source));
        }

        private AudioSource GetPooledAudioSource(AudioConfig config)
        {
            foreach (var source in _audioSourcePool)
            {
                if (!source.isPlaying)
                {
                    SetupSource(source, config);
                    source.gameObject.SetActive(true);
                    return source;
                }
            }

            AudioSource newSource = CreateNewAudioSource(config);
            _audioSourcePool.Add(newSource);
            return newSource;
        }

        private IEnumerator HandleSourceReturn(AudioSource source)
        {
            yield return new WaitForSeconds(source.clip.length);

            if (!source.loop)
            {
                source.Stop();
                source.gameObject.SetActive(false);
            }
        }

        private AudioSource CreateNewAudioSource(AudioConfig config)
        {
            GameObject go = new GameObject("AudioSource");
            go.transform.SetParent(transform);
            AudioSource source = go.AddComponent<AudioSource>();

            SetupSource(source, config);
            return source;
        }

        private void SetupSource(AudioSource source, AudioConfig config)
        {
            source.clip = config.Clip;
            source.loop = config.Loop;
            source.volume = config.Volume;
            source.playOnAwake = false;

            source.outputAudioMixerGroup = config.IsMusic ? musicGroup : sfxGroup;
        }

        /// <summary>
        /// Sets the master volume in the audio mixer using a normalized value (0–1).
        /// Intended for binding with a UI slider.
        /// </summary>
        public void SetMasterVolume(float volume) => mainMixer.SetFloat(masterVolume, ConvertToDecibels(volume));

        /// <summary>
        /// Sets the music volume in the audio mixer using a normalized value (0–1).
        /// Intended for binding with a UI slider.
        /// </summary>
        public void SetMusicVolume(float volume) => mainMixer.SetFloat(musicVolume, ConvertToDecibels(volume));

        /// <summary>
        /// Sets the SFX volume in the audio mixer using a normalized value (0–1).
        /// Intended for binding with a UI slider.
        /// </summary>
        public void SetSFXVolume(float volume) => mainMixer.SetFloat(sfxVolume, ConvertToDecibels(volume));

        /// <summary>
        /// Converts a normalized volume (0–1) into decibel scale for the audio mixer.
        /// </summary>
        private float ConvertToDecibels(float volume)
        {
            return volume > 0 ? Mathf.Log10(volume) * 20 : -80f;
        }

        private void ValidateReferences()
        {
            ReferenceValidator.Validate(audioManagerDataSource, nameof(audioManagerDataSource), this);
            ReferenceValidator.Validate(audioLibrary, nameof(audioLibrary), this);
            ReferenceValidator.Validate(onPlayAudio, nameof(onPlayAudio), this);

            ReferenceValidator.Validate(mainMixer, nameof(mainMixer), this);
            ReferenceValidator.Validate(musicGroup, nameof(musicGroup), this);
            ReferenceValidator.Validate(sfxGroup, nameof(sfxGroup), this);
        }
    }
}
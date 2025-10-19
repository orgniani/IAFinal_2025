using System;
using UnityEngine;

namespace Audio
{
    /// <summary>
    /// Defines configuration for a single audio clip,
    /// including volume, looping, and whether it's music or SFX.
    /// </summary>
    [CreateAssetMenu(menuName = "Config/Audio", fileName = "AudioCfg", order = 0)]
    public class AudioConfig : ScriptableObject
    {
        [field: SerializeField] public AudioClip Clip { get; private set; }

        [field: SerializeField] public bool Loop { get; private set; }

        [field: SerializeField, Range(0f, 1f)] public float Volume { get; private set; } = 1f;

        [field: SerializeField] public bool IsMusic { get; private set; } = false;

        private void OnValidate()
        {
            if (Clip == null)
                Debug.LogError($"[AudioConfig] Audio clip for {name} is null!");
        }
    }
}

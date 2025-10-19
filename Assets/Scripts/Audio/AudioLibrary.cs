using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

namespace Audio
{
    [CreateAssetMenu(menuName = "Library/Audio", fileName = "AudioLibrary", order = 0)]
    public class AudioLibrary : ScriptableObject
    {
        [Serializable]
        public class AudioEntry
        {
            public AudioKey Key;
            public AudioConfig Config;
        }

        [SerializeField] private List<AudioEntry> audioEntries = new();
        private Dictionary<AudioKey, AudioConfig> _lookup;

        private void OnEnable()
        {
            _lookup = audioEntries.ToDictionary(e => e.Key, e => e.Config);
        }

        /// <summary>
        /// Returns the AudioConfig assigned to the given AudioKey, or null if not found.
        /// </summary>
        public AudioConfig GetConfig(AudioKey key)
        {
            return _lookup.TryGetValue(key, out var config) ? config : null;
        }

        /// <summary>
        /// Validates the audio entry list to ensure:
        /// - No duplicate keys
        /// - All enum values are represented
        /// - No null configs
        /// </summary>
        private void OnValidate()
        {
            var duplicates = audioEntries
                .GroupBy(e => e.Key)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicates.Count > 0)
            {
                Debug.LogError($"[AudioLibrary] Duplicate audio keys found: {string.Join(", ", duplicates)}");
            }

            var definedKeys = audioEntries.Select(e => e.Key).ToHashSet();
            var allKeys = Enum.GetValues(typeof(AudioKey)).Cast<AudioKey>();

            var missingKeys = allKeys.Except(definedKeys).ToList();
            if (missingKeys.Count > 0)
            {
                Debug.LogWarning($"[AudioLibrary] Missing audio entries for keys: {string.Join(", ", missingKeys)}");
            }

            foreach (var entry in audioEntries)
            {
                if (entry.Config == null)
                    Debug.LogError($"[AudioLibrary] AudioConfig for key '{entry.Key}' is null.");
            }
        }
    }
}

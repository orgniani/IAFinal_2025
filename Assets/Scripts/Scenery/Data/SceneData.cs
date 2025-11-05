using UnityEngine;
using System.Linq;

namespace Scenery.Data
{
    /// <summary>
    /// Represents a named group of scene indexes used for loading/unloading.
    /// Used by FlowManager and SceneryManager to define which scenes belong to
    /// levels, menus, managers or other groups.
    /// </summary>
    [CreateAssetMenu(menuName = "Scenes/Scene Data", fileName = "_SceneData", order = 0)]
    public class SceneData : ScriptableObject
    {
        [SerializeField] private string logName;
        [field: SerializeField] public int[] SceneIndexes { get; private set; }

        /// <summary>
        /// Validates that SceneIndexes is properly configured and free of duplicates.
        /// Called automatically by Unity when asset is modified or saved.
        /// </summary>
        private void OnValidate()
        {
            if (SceneIndexes == null || SceneIndexes.Length == 0)
            {
                Debug.LogError($"{logName}: the array of {nameof(SceneIndexes)} is empty!");
                return;
            }

            var duplicates = SceneIndexes
                .GroupBy(x => x)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicates.Count > 0)
            {
                Debug.LogError($"{logName}: Duplicate scene indexes found: {string.Join(", ", duplicates)}");
            }
        }
    }
}

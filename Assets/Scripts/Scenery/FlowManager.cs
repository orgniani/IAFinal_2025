using System.Collections;
using UnityEngine;
using Scenery.Data;
using DataSource;
using Helpers;

namespace Scenery
{
    /// <summary>
    /// Controls the game's scene flow:
    /// - Initializes the SceneryManager with the scene layout.
    /// - Loads the initial permanent batch (menus, managers).
    /// - Manages level loading, unloading, restarting, and progression.
    /// </summary>
    public class FlowManager : MonoBehaviour
    {
        [Header("Data Sources")]
        [SerializeField] private DataSource<FlowManager> flowManagerDataSource;

        [Header("Managers")]
        [SerializeField] private SceneryManager sceneryManager;

        [Header("Scenes")]
        [Tooltip("Contains scene indexes for the Menus and Managers.\n" +
                 "This batch must include all persistent scenes that should NEVER be unloaded.")]
        [SerializeField] private SceneData persistentBatch;

        [Tooltip("Contains scene indexes for each level.\n" +
                 "Each SceneryLoadId corresponds to a level and is loaded/unloaded as needed.")]
        [SerializeField] private SceneData[] levels;

        private SceneData[] _allSceneIds;

        private int _levelsAmount = 0;
        private int _currentLevelIndex;
        private int _lastPlayedLevelIndex = 0;

        private void Awake()
        {
            Application.targetFrameRate = 60;

            ValidateReferences();
            CombineSceneryIds();
            sceneryManager.SetUp(_allSceneIds, persistentBatch.SceneIndexes);

            Debug.unityLogger.logEnabled = true;
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
            Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
            Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.None);

        }

        private void OnEnable()
        {
            if (!flowManagerDataSource.DataInstance)
                flowManagerDataSource.DataInstance = this;
        }

        private IEnumerator Start()
        {
            // Forcefully unload every loaded scene at startup
            foreach (var id in _allSceneIds)
                sceneryManager.ForceUnloadScenes(id.SceneIndexes);

            // Wait a frame to ensure unloads are processed
            yield return null;

            // Load first batch
            sceneryManager.HandleLoadScenery(persistentBatch.SceneIndexes);
        }

        private void OnDisable()
        {
            if (flowManagerDataSource.DataInstance == this)
                flowManagerDataSource.DataInstance = null;
        }

        private void CombineSceneryIds()
        {
            _levelsAmount = levels.Length;

            // Total SceneryLoadId entries = 1 for persistent batch + all levels
            _allSceneIds = new SceneData[1 + _levelsAmount];
            _allSceneIds[0] = persistentBatch;

            for (int i = 0; i < _levelsAmount; i++)
                _allSceneIds[i + 1] = levels[i];
        }

        [ContextMenu("Load Current Level")]
        public void LoadCurrentLevel()
        {
            if (_currentLevelIndex >= _levelsAmount)
            {
                Debug.LogWarning($"{name}: No more levels to load!" +
                                 $"\nCurrent level index: {_currentLevelIndex}." +
                                 $"\nTotal levels: {_levelsAmount}.");

                ResetToFirstLevel();
            }

            _lastPlayedLevelIndex = _currentLevelIndex;

            var levelScenes = levels[_currentLevelIndex].SceneIndexes;
            sceneryManager.HandleLoadScenery(levelScenes);
        }

        [ContextMenu("Unload Current Level")]
        public void UnloadCurrentLevel()
        {
            var levelScenes = levels[_lastPlayedLevelIndex].SceneIndexes;
            sceneryManager.HandleUnloadScenery(levelScenes);
        }

        [ContextMenu("Reload Current Level")]
        public void ReloadCurrentLevel()
        {
            LoadCurrentLevel();
        }

        [ContextMenu("Reset To First Level")]
        public void ResetToFirstLevel()
        {
            _currentLevelIndex = 0;
        }

        private void ValidateReferences()
        {
            ReferenceValidator.Validate(flowManagerDataSource, nameof(flowManagerDataSource), this);
            ReferenceValidator.Validate(sceneryManager, nameof(sceneryManager), this);
            ReferenceValidator.Validate(persistentBatch, nameof(persistentBatch), this);

            foreach (SceneData level in levels)
                ReferenceValidator.Validate(level, nameof(level), this);

            if (levels.Length <= 0)
            {
                Debug.LogError($"{name}: the array of {nameof(levels)} is empty!" +
                               $"\nDisabling component to avoid errors.");
                enabled = false;
                return;
            }
        }
    }
}
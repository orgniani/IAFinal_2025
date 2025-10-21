using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Events;
using System;
using System.Linq;
using Scenery.Data;
using Helpers;

namespace Scenery
{
    public class SceneryManager : MonoBehaviour
    {
        [Header("Parameters")]
        [Tooltip("Artificial delay after scene load/unload to ensure UI animations have time to play.")]
        [SerializeField] private float fakeLoadingTime = 1;

        [Header("Invoke events")]
        [SerializeField] private EmptyAction onLoadStart;
        [SerializeField] private FloatAction onLoadProgress;
        [SerializeField] private EmptyAction onLoadEnd;

        [Header("Logs")]
        [SerializeField] private bool enableLogs = true;

        /// <summary>
        /// Tracks the currently loaded level scene indexes.
        /// Used to determine what should be unloaded before loading new scenes.
        /// </summary>
        private int[] _currentLevelIds;
        private HashSet<int> _protectedSceneIndexes = new HashSet<int>();

        /// <summary>
        /// Initializes the scenery manager with all available scene configurations.
        /// </summary>
        /// <param name="sceneryLoadIds">Array of all the SceneryLoadId assets involved in the game.</param>
        /// <param name="protectedScenes">Scene indexes that should never be unloaded.</param>
        public void SetUp(SceneData[] sceneryLoadIds, int[] protectedScenes)
        {
            ValidateReferences();
            _currentLevelIds = new int[0];
            _protectedSceneIndexes = new HashSet<int>(protectedScenes);
        }

        /// <summary>
        /// Force-unloads any scenes, including protected ones. Used only at startup for testing purposes.
        /// </summary>
        public void ForceUnloadScenes(int[] sceneIndexes)
        {
            StartCoroutine(Unload(sceneIndexes, true));
        }

        /// <summary>
        /// Loads a new batch of scenes, automatically unloading currently tracked scenes.
        /// </summary>
        public void HandleLoadScenery(int[] scenesToLoad)
        {
            StartCoroutine(UnloadAndLoadScenes(_currentLevelIds ?? Array.Empty<int>(), scenesToLoad));
            _currentLevelIds = scenesToLoad;
        }

        /// <summary>
        /// Unloads a set of scenes without loading new ones.
        /// </summary>
        public void HandleUnloadScenery(int[] scenesToUnload)
        {
            StartCoroutine(UnloadAndLoadScenes(scenesToUnload, Array.Empty<int>()));
            if (_currentLevelIds != null && _currentLevelIds.SequenceEqual(scenesToUnload))
                _currentLevelIds = Array.Empty<int>();
        }

        /// <summary>
        /// Handles unloading scenes followed by loading new ones.
        /// Includes artificial delays for smoother visual feedback.
        /// </summary>
        private IEnumerator UnloadAndLoadScenes(int[] unloadSceneIndexes, int[] loadSceneIndexes)
        {
            onLoadStart?.InvokeEvent();
            yield return new WaitForSeconds(fakeLoadingTime);

            float unloadProgress = 0f;
            float loadProgress = 0f;
            int activePhases = 0;
            if (unloadSceneIndexes.Length > 0) activePhases++;
            if (loadSceneIndexes.Length > 0) activePhases++;
            if (activePhases == 0) activePhases = 1;

            if (unloadSceneIndexes.Length > 0)
            {
                yield return Unload(unloadSceneIndexes, false, progress =>
                {
                    unloadProgress = progress;
                    float totalProgress = (unloadProgress / activePhases);
                    onLoadProgress?.InvokeEvent(totalProgress);
                });
            }
            else
            {
                unloadProgress = 1f;
            }

            if (loadSceneIndexes.Length > 0)
            {
                yield return Load(loadSceneIndexes, progress =>
                {
                    loadProgress = progress;
                    float totalProgress = ((unloadProgress + loadProgress) / activePhases);
                    onLoadProgress?.InvokeEvent(totalProgress);
                });
            }
            else
            {
                loadProgress = 1f;
            }

            onLoadProgress?.InvokeEvent(1f);
            yield return new WaitForSeconds(fakeLoadingTime);

            _currentLevelIds = loadSceneIndexes;
            onLoadEnd?.InvokeEvent();
        }

        /// <summary>
        /// Loads scenes additively from the provided list.
        /// Reports progress during the process.
        /// </summary>
        private IEnumerator Load(int[] sceneIndexes, Action<float> onProgress = null)
        {
            int totalScenes = sceneIndexes.Length;
            int completedScenes = 0;

            foreach (var sceneIndex in sceneIndexes)
            {
                var loadOp = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);

                if (loadOp == null)
                {
                    if (enableLogs) Debug.LogError($"Failed to load scene at index {sceneIndex}");
                    continue;
                }

                loadOp.allowSceneActivation = true;

                while (!loadOp.isDone)
                {
                    float totalProgress = (completedScenes + loadOp.progress) / totalScenes;
                    onProgress?.Invoke(totalProgress);
                    yield return null;
                }

                completedScenes++;
                onProgress?.Invoke((float)completedScenes / totalScenes);
            }
        }

        /// <summary>
        /// Unloads scenes from the provided list if allowed.
        /// Reports progress during the process.
        /// </summary>
        private IEnumerator Unload(int[] sceneIndexes, bool force = false, Action<float> onProgress = null)
        {
            int totalScenes = sceneIndexes.Length;
            int completedScenes = 0;

            foreach (var sceneIndex in sceneIndexes)
            {
                if (!force && _protectedSceneIndexes.Contains(sceneIndex))
                {
                    if (enableLogs) Debug.Log($"Skipping unload of protected scene at index {sceneIndex}");
                    completedScenes++;
                    onProgress?.Invoke((float)completedScenes / totalScenes);
                    continue;
                }

                if (SceneManager.GetSceneByBuildIndex(sceneIndex).isLoaded)
                {
                    var unloadOp = SceneManager.UnloadSceneAsync(sceneIndex);

                    if (unloadOp == null)
                    {
                        if (enableLogs) Debug.LogError($"Failed to unload scene at index {sceneIndex}");
                        completedScenes++;
                        onProgress?.Invoke((float)completedScenes / totalScenes);
                        continue;
                    }

                    while (!unloadOp.isDone)
                    {
                        float totalProgress = (completedScenes + unloadOp.progress) / totalScenes;
                        onProgress?.Invoke(totalProgress);
                        yield return null;
                    }

                    completedScenes++;
                    onProgress?.Invoke((float)completedScenes / totalScenes);
                }
                else
                {
                    if (enableLogs)
                        Debug.Log($"<color=purple> Scene at index {sceneIndex} is not currently loaded. Skipping unload operation. </color>");

                    completedScenes++;
                    onProgress?.Invoke((float)completedScenes / totalScenes);
                }
            }
        }

        private void ValidateReferences()
        {
            ReferenceValidator.Validate(onLoadStart, nameof(onLoadStart), this);
            ReferenceValidator.Validate(onLoadEnd, nameof(onLoadEnd), this);
            ReferenceValidator.Validate(onLoadProgress, nameof(onLoadProgress), this);
        }
    }
}

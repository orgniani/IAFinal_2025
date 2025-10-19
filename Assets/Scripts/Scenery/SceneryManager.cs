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

        public void HandleUnloadScenery(int[] scenesToUnload)
        {
            StartCoroutine(UnloadAndLoadScenes(scenesToUnload, Array.Empty<int>()));

            // If these are the current level, clear current level tracker
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

            if (unloadSceneIndexes.Length > 0)
                yield return Unload(unloadSceneIndexes, false);

            if (loadSceneIndexes.Length > 0)
                yield return Load(loadSceneIndexes);

            yield return new WaitForSeconds(fakeLoadingTime);

            _currentLevelIds = loadSceneIndexes;
            onLoadEnd?.InvokeEvent();
        }

        /// <summary>
        /// Loads scenes additively from the provided list.
        /// </summary>
        private IEnumerator Load(int[] sceneIndexes)
        {
            foreach (var sceneIndex in sceneIndexes)
            {
                var loadOp = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);

                if (loadOp == null)
                {
                    if (enableLogs) Debug.LogError($"Failed to load scene at index {sceneIndex}");
                    continue;
                }

                while (!loadOp.isDone)
                    yield return null;
            }
        }

        /// <summary>
        /// Unloads scenes from the provided list if allowed.
        /// </summary>
        private IEnumerator Unload(int[] sceneIndexes, bool force = false)
        {
            foreach (var sceneIndex in sceneIndexes)
            {
                if (!force && _protectedSceneIndexes.Contains(sceneIndex))
                {
                    if (enableLogs) Debug.Log($"Skipping unload of protected scene at index {sceneIndex}");
                    continue;
                }

                if (SceneManager.GetSceneByBuildIndex(sceneIndex).isLoaded)
                {
                    var unloadOp = SceneManager.UnloadSceneAsync(sceneIndex);

                    if (unloadOp == null)
                    {
                        if (enableLogs) Debug.LogError($"Failed to unload scene at index {sceneIndex}");
                        continue;
                    }

                    while (!unloadOp.isDone)
                        yield return null;
                }

                else
                {
                    if (enableLogs)
                    {
                        Debug.Log($"<color=purple> Scene at index {sceneIndex} is not currently loaded.\n" +
                                  $"Skipping unload operation. </color>");
                    }
                }
            }
        }

        private void ValidateReferences()
        {
            ReferenceValidator.Validate(onLoadStart, nameof(onLoadStart), this);
            ReferenceValidator.Validate(onLoadEnd, nameof(onLoadEnd), this);
        }
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;
using Oryx.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using SM = UnityEngine.SceneManagement;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Oryx
{

    public class ScenesTransitionManager
    {

        public GameEvent<SceneData> SceneClosing = new GameEvent<SceneData>();
        public GameEvent<SceneData> SceneLoading = new GameEvent<SceneData>();
        public GameEvent<SceneData> SceneOpening = new GameEvent<SceneData>();
        public GameEvent<SceneData> SceneOpened = new GameEvent<SceneData>();
        public GameEvent<float> SceneLoadingProgress = new GameEvent<float>();

        public SceneData ActiveScene
        {
            get
            {
                return _activeScene;
            }
        }
        
        [SerializeField] private SceneData[] _scenesData;


        private SceneData _activeScene;
        private List<string> _currentAdditiveScenes = new List<string>();
        
        private static bool _loading;

        public bool TransitionToScene(SceneType sceneType, SceneData[] additionalScenesToLoad = null)
        {
            SceneData sceneData = null;
            foreach (var data in _scenesData)
            {
                if (data.SceneType == sceneType && data.LoadMode == SM.LoadSceneMode.Single)
                {
                    sceneData = data;
                    break;
                }
            }

            if (sceneData == null)
                return false;
            
            return TransitionToScene(sceneData, additionalScenesToLoad);
        }

        /// <returns>True if transition have successfully be started (ie. there is no other transitions).</returns>
        public bool TransitionToScene(SceneData sceneData, SceneData[] additionalScenesToLoad = null)
        {
            if (!_loading)
            {
                TransitioningTo(sceneData, additionalScenesToLoad);
            } else return false;
        
            return true;
        }

        public async void LoadAdditiveScene(SceneData sceneData, bool openWithTransition)
        {
            if (openWithTransition)
                await TransitioningTo(sceneData);
            else
            {
                _loading = true;
                await LoadSceneInternal(sceneData);
                _currentAdditiveScenes.Add(sceneData.SceneName);
                
                SceneOpened.Invoke(sceneData);
                _loading = false;
            }
        }
        
        public async void UnloadAdditiveScene(SceneData sceneData)
        {
            if (sceneData.LoadMode == SM.LoadSceneMode.Single)
                return;
            
            var operation = UnitySceneManager.UnloadSceneAsync(sceneData.SceneName);
            while (!operation.isDone)
            {
                await TaskUtils.WaitFrame();
            }
            _currentAdditiveScenes.Remove(sceneData.SceneName);
        }

        private async Task<bool> TransitioningTo(SceneData sceneData, SceneData[] additionalScenesToLoad = null)
        {
            _loading = true;

            if (_activeScene != null)
            {
                SceneClosing.Invoke(_activeScene);
                await TaskUtils.WaitForSecondsAsync(_activeScene.ClosingDuration);

                SceneLoading.Invoke(_activeScene);
            }

            
            if (sceneData.LoadMode == SM.LoadSceneMode.Single)
            {
                _activeScene = sceneData;
                _currentAdditiveScenes.Clear();
            }

            await LoadSceneInternal(sceneData);
            
            foreach (var additive in sceneData.AdditiveScenes)
            {
                if(_currentAdditiveScenes.Contains(additive.SceneName))
                    continue;

                await LoadSceneInternal(additive);
                _currentAdditiveScenes.Add(additive.SceneName);
            }

            if (additionalScenesToLoad != null && additionalScenesToLoad.Length > 0)
            {
                foreach (var additive in additionalScenesToLoad)
                {
                    await LoadSceneInternal(additive);
                }
            }

            SceneOpening.Invoke(sceneData);

            await TaskUtils.WaitForSecondsAsync(sceneData.InitDuration);
            SceneOpened.Invoke(sceneData);

            _loading = false;
            return true;
        }

        private async Task<SM.Scene> LoadSceneInternal(SceneData sceneData)
        {
            var operation = UnitySceneManager.LoadSceneAsync(sceneData.SceneName, sceneData.LoadMode);

            while (!operation.isDone)
            {
                await TaskUtils.WaitFrame();
                SceneLoadingProgress.Invoke(operation.progress);
            }
            var scene = UnitySceneManager.GetSceneByName(sceneData.SceneName);
            if (sceneData.ShouldBeActiveScene)
            {
                UnitySceneManager.SetActiveScene(scene);
            }

            return scene;
        }

    }
}

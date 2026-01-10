using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Oryx
{
    /// <summary>
    /// REMINDER - INHERIT FROM THIS SCRIPTABLE OBJECT FOR EACH GAME-SPECIFIC IMPLEMENTATION
    /// </summary>
    public class SceneData : ScriptableObject
    {
        public SceneType SceneType = SceneType.Game;
        public string SceneName;
        public LoadSceneMode LoadMode;
        public bool ShouldBeActiveScene;
        public float InitDuration;
        public float ClosingDuration;

        public SceneData[] AdditiveScenes;
        

#if UNITY_EDITOR
        private bool IsSceneValid(string scene)
        {
            if (scene == "") return false;
            Scene Scene = UnityEditor.SceneManagement.EditorSceneManager.GetSceneByName(scene);
            Debug.Log(Scene.IsValid() + " " + Scene.buildIndex);
            return Scene.IsValid() && Scene.buildIndex < SceneManager.sceneCount && Scene.buildIndex >= 0;
        }
#endif
    }
    
    public enum SceneType
    {
        Loading,
        MainMenu,
        Game,
        Level
    }
}



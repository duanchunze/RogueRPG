using System;
using YooAsset;
#if UNITY_EDITOR
using UnityEngine;
#endif

namespace Hsenl {
    public static partial class SceneManager {
        public static event Action<Scene> OnUnitySceneLoaded;

        static SceneManager() { }

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize() {
            UnloadAllScene();
        }
#endif

        public static async HTask<Scene> LoadSceneWithUnity(string name, LoadSceneMode loadSceneMode) {
            var scene = LoadScene(name, loadSceneMode);
            await YooAssets.LoadSceneAsync(name, (UnityEngine.SceneManagement.LoadSceneMode)(int)loadSceneMode);
            OnUnitySceneLoaded?.Invoke(scene);
            return scene;
        }

        public static void MoveEntityToSceneWithUnity(Entity entity, Scene scene) {
            MoveEntityToScene(entity, scene);

            var go = entity.GameObject;
            switch (scene.sceneType) {
                case SceneType.Normal:
                    var monoScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(scene.sceneName);
                    if (monoScene == default) return;
                    UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(go, monoScene);
                    break;
                case SceneType.DontDestroyOnLoad:
                    UnityEngine.Object.DontDestroyOnLoad(go);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
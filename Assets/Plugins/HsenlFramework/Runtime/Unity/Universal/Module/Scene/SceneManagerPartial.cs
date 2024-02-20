using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

namespace Hsenl {
    public static partial class SceneManager {
        public static event Action<Scene> OnUnitySceneLoaded;

        static SceneManager() { }

        [OnEventSystemInitialized]
        private static void Initialize() {
            UnloadAllScene();
        }

        // public static async ETTask<Scene> LoadSceneWithUnity(string name, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode) {
        //     var scene = LoadScene(name, (LoadSceneMode)(int)loadSceneMode);
        //     if (ResourcesManager.Instance.EditorMode) {
        //         var paths = AssetBundleHelper.GetAssetPathsFromAssetBundle(name.ToBundleName());
        //         if (paths.Length == 0) return null;
        //         UnityEngine.SceneManagement.SceneManager.LoadScene(paths[0], loadSceneMode);
        //         await Timer.WaitFrame();
        //     }
        //     else {
        //         if (!ResourcesManager.Contains(name.ToBundleName())) {
        //             await ResourcesManager.LoadBundleAsync(name.ToBundleName());
        //         }
        //
        //         var operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name, loadSceneMode);
        //         await operation;
        //     }
        //
        //     try {
        //         OnUnitySceneLoaded?.Invoke(scene);
        //     }
        //     catch (Exception e) {
        //         Log.Error(e);
        //     }
        //
        //     return scene;
        // }
        
        public static async HTask<Scene> LoadSceneWithUnity(string name, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode) {
            var scene = LoadScene(name, (LoadSceneMode)(int)loadSceneMode);
            await YooAssets.LoadSceneAsync(name);
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
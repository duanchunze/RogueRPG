using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hsenl {
    public static partial class SceneManager {
        public static event Action<Scene> OnUnitySceneLoaded;

        static SceneManager() { }

#if UNITY_EDITOR
        // 这个方法是为了配合unity编辑器里的一个设置, 当我们把unity的reload domain设置勾掉之后, 我们就不会每次运行游戏, 都重新编译一遍代码, 这样效率虽然快了,
        // 但不重载代码也意为着原来的数据也没有被清除, 所以, 我们需要手动的去清除. 也因为这个问题只在编辑器发生, 打包后的程序不存在这个问题, 所以使用 UNITY_EDITOR 宏.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize() {
            UnloadAllScene();
        }
#endif



        public static async ETTask<Scene> LoadSceneWithUnity(string name, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode) {
            var scene = LoadScene(name, (LoadSceneMode)(int)loadSceneMode);
            if (ResourcesManager.Instance.EditorMode) {
                var paths = AssetBundleHelper.GetAssetPathsFromAssetBundle(name.ToBundleName());
                if (paths.Length == 0) return null;
                UnityEngine.SceneManagement.SceneManager.LoadScene(paths[0], loadSceneMode);
                await Timer.WaitFrame();
            }
            else {
                if (!ResourcesManager.Contains(name.ToBundleName())) {
                    await ResourcesManager.LoadBundleAsync(name.ToBundleName());
                }

                var operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name, loadSceneMode);
                await operation;
            }

            try {
                OnUnitySceneLoaded?.Invoke(scene);
            }
            catch (Exception e) {
                Log.Error(e);
            }

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
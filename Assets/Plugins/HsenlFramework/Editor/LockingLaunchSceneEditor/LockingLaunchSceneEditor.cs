using UnityEditor;
using UnityEngine;

namespace Hsenl {
    public static class LockingLaunchSceneEditor {
        private const string MenuKey = "Hsenl/锁定启动场景";
        private const string prefsKey = "lock launch scene";
        private const string LaunchScene = "Launch";

        private static bool _locked;

        [InitializeOnLoadMethod]
        private static void Initialize() {
            _locked = EditorPrefs.GetBool(prefsKey, false);
            Menu.SetChecked(MenuKey, _locked);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Entry() {
            if (!_locked) return;
            var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (activeScene.name == LaunchScene) return;

            // unity运行时默认加载入口场景使用的是同步加载方式, 这代表我们无法中断正在加载中的该场景, 所以我们只能把所有根物体禁用掉, 以让他即便加载完后, 也不起作用(包括Awake也不会触发)
            foreach (var rootGameObject in activeScene.GetRootGameObjects()) {
                rootGameObject.SetActive(false);
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene(LaunchScene, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }

        [MenuItem(MenuKey, priority = int.MaxValue - 1)]
        private static void SetLock() {
            if (_locked) {
                Debug.Log("已解锁启动场景");
                EditorApplication.UnlockReloadAssemblies();
            }
            else {
                Debug.Log($"已锁定{LaunchScene}为启动场景");
                EditorApplication.UnlockReloadAssemblies();
            }

            _locked = !_locked;
            EditorPrefs.SetBool(prefsKey, _locked);
            Menu.SetChecked(MenuKey, _locked);
        }
    }
}
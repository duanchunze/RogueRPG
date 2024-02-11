using UnityEditor;
using UnityEngine;

namespace Hsenl {
    [InitializeOnLoad]
    public class LockEntryEditor {
        private const string MenuKey = "ET/锁定入口";
        private const string prefsKey = "lock entry scene";
        private static bool _locked;

        static LockEntryEditor() {
            _locked = EditorPrefs.GetBool(prefsKey, false);
            Menu.SetChecked(MenuKey, _locked);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Entry() {
            if (!_locked) return;
            var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (activeScene.name == "Launch") return;
            
            // unity运行时默认加载入口场景使用的是同步加载方式, 这代表我们无法中断正在加载中的该场景, 所以我们只能把所有根物体禁用掉, 以让他即便加载完后, 也不起作用(包括Awake也不会触发)
            foreach (var rootGameObject in activeScene.GetRootGameObjects()) {
                rootGameObject.SetActive(false);
            }
            
            UnityEngine.SceneManagement.SceneManager.LoadScene("Launch", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }

        [MenuItem(MenuKey, priority = int.MaxValue - 1)]
        private static void SetLock() {
            if (_locked) {
                Debug.Log("已解锁入口场景");
                EditorApplication.UnlockReloadAssemblies();
                _locked = !_locked;
                EditorPrefs.SetBool(prefsKey, _locked);
                Menu.SetChecked(MenuKey, _locked);
            }
            else {
                Debug.Log("已锁定Entry为入口场景");
                EditorApplication.UnlockReloadAssemblies();
                _locked = !_locked;
                EditorPrefs.SetBool(prefsKey, _locked);
                Menu.SetChecked(MenuKey, _locked);
            }
        }
    }
}
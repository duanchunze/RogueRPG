using UnityEditor;
using UnityEngine;

namespace Hsenl {
    [InitializeOnLoad]
    internal class CustomScriptCompile {
        private const string MenuKey = "ET/脚本/锁定重新加载程序集";
        private const string Key = "LockReloadAssemblies";
        private static bool _lockReloadAssemblies;

        static CustomScriptCompile() {
            _lockReloadAssemblies = EditorPrefs.GetBool(Key, false);
            Menu.SetChecked(MenuKey, _lockReloadAssemblies);
            if (_lockReloadAssemblies) {
                EditorApplication.LockReloadAssemblies();
            }
            else {
                EditorApplication.UnlockReloadAssemblies();
            }

            // EditorApplication.playModeStateChanged += LogPlayModeState;
        }

        // private static void LogPlayModeState(PlayModeStateChange state) {
        //     if (state == PlayModeStateChange.EnteredPlayMode && EditorPrefs.GetBool(Key, false)) {
        //         EditorApplication.isPlaying = false;
        //         Debug.LogWarning("重新加载程序集已被锁定。");
        //         EditorUtility.DisplayDialog("警告", "已锁定重新加载程序集，请注意！！！", "知道后果");
        //     }
        // }
        
        [MenuItem(MenuKey, priority = int.MaxValue)]
        private static void SetLockReloadAssemblies() {
            if (_lockReloadAssemblies) {
                Debug.Log("重新加载程序集已解锁");
                EditorApplication.UnlockReloadAssemblies();
                _lockReloadAssemblies = !_lockReloadAssemblies;
                EditorPrefs.SetBool(Key, false);
                Menu.SetChecked(MenuKey, false);
            }
            else {
                if (EditorUtility.DisplayDialog("提示", "是否锁定 重新加载程序集 \n\n锁定以后无法重新加载程序集,\n也不会触发脚本编译。", "继续锁定", "取消")) {
                    Debug.Log("重新加载程序集已锁定");
                    EditorApplication.LockReloadAssemblies();
                    _lockReloadAssemblies = !_lockReloadAssemblies;
                    EditorPrefs.SetBool(Key, true);
                    Menu.SetChecked(MenuKey, true);
                }
            }
        }

        [MenuItem("ET/脚本/编译脚本 %w")]
        private static void CompileScript() {
            AssetDatabase.Refresh();
            EditorApplication.UnlockReloadAssemblies();
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
        }
    }
}
using UnityEditor;
using UnityEngine;

namespace Hsenl {
    /*
     * 可以阻止unity的编译, 注意是"阻止", 而不是"关闭", 要关闭需要去编辑器手动关闭.
     * 目的是阻止unity的每次编译导致的浪费时间. 相比从编辑器直接关闭自动编译, 这种方式的优点是, 即便你开启了"阻止编译", 当你运行的时候, 也会强制的进行编译再运行
     */
    public static class LockingScriptCompilationEditor {
        private const string MenuKey = "Hsenl/锁定程序集重载/锁定";
        private const string Key = "LockReloadAssemblies";
        private static bool _lockReloadAssemblies;
        
        [InitializeOnLoadMethod]
        private static void Initialize() {
            _lockReloadAssemblies = EditorPrefs.GetBool(Key, false);
            Menu.SetChecked(MenuKey, _lockReloadAssemblies);
            if (_lockReloadAssemblies) {
                EditorApplication.LockReloadAssemblies();
            }
            else {
                EditorApplication.UnlockReloadAssemblies();
            }
        }
        
        [MenuItem(MenuKey, priority = int.MaxValue - 1)]
        private static void SetLockReloadAssemblies() {
            if (_lockReloadAssemblies) {
                Debug.Log("重新加载程序集已解锁");
                EditorApplication.UnlockReloadAssemblies();
                _lockReloadAssemblies = !_lockReloadAssemblies;
                EditorPrefs.SetBool(Key, false);
                Menu.SetChecked(MenuKey, false);
            }
            else {
                if (EditorUtility.DisplayDialog("提示", "是否锁定 重新加载程序集 \n\n锁定后, 以后的编译, 我们需要手动编译", "继续锁定", "取消")) {
                    Debug.Log("重新加载程序集已锁定");
                    EditorApplication.LockReloadAssemblies();
                    _lockReloadAssemblies = !_lockReloadAssemblies;
                    EditorPrefs.SetBool(Key, true);
                    Menu.SetChecked(MenuKey, true);
                }
            }
        }

        [MenuItem("Hsenl/锁定程序集重载/手动编译脚本 %w")]
        private static void CompileScript() {
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            EditorApplication.UnlockReloadAssemblies();
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
        }
    }
}
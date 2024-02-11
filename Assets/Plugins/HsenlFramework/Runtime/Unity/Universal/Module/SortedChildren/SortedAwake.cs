using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Hsenl {
    // 挂在此组件下的组件, 会按照从上到下的顺序, 依次被激活
    // 唯一需要注意的是, 把该脚本的执行顺序放到目标脚本的前面
    // 
    // 原理: 赶在所有的子物体被唤醒前, 将其关闭, 然后再Start里, 统一按顺序开启. 因为是被唤醒前就被关闭了, 所以类似 OnDisable 也不会触发
    // 优点: 对于子物体来说, 不需要做任何额外操作, 也没有任何影响
    // 缺点: 需要对脚本执行顺序进行设置, 迁移脚本后, 可能需要对项目重新设置, but, 后来知道了下面这个api, 就不用麻烦的从编辑器设置了, 所以现在, 没什么缺点了
    [DefaultExecutionOrder(-10)]
    [DisallowMultipleComponent]
    public class SortedAwake : MonoBehaviour {
        public int delayAwake;

        public UnityEvent onAwakeDone;

        private HashSet<GameObject> _inactives = new();

        private void Awake() {
            for (int i = 0, len = this.transform.childCount; i < len; i++) {
                var child = this.transform.GetChild(i);
                if (child.GetComponent<SortedAwakeIgnore>()) continue;
                var go = child.gameObject;
                if (!go.activeSelf) {
                    this._inactives.Add(go);
                    continue;
                }

                go.SetActive(false);
            }
        }

        private IEnumerator Start() {
            var delay = this.delayAwake;
            while (delay > 0) {
                delay--;
                yield return null;
            }

            for (int i = 0, len = this.transform.childCount; i < len; i++) {
                var child = this.transform.GetChild(i);
                if (child.GetComponent<SortedAwakeIgnore>()) continue;
                if (this._inactives.Contains(child.gameObject)) continue;
                var sortedAwake = child.GetComponent<SortedAwake>();
                if (sortedAwake != null) {
                    sortedAwake.gameObject.SetActive(true);
                }
            }

            for (int i = 0, len = this.transform.childCount; i < len; i++) {
                var child = this.transform.GetChild(i);
                if (child.GetComponent<SortedAwakeIgnore>()) continue;
                if (this._inactives.Contains(child.gameObject)) continue;
                child.gameObject.SetActive(true);
            }

            yield return null;

            this.onAwakeDone?.Invoke();
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(SortedAwake))]
    public class SortedAwakeEditor : Editor {
        private SortedAwake _sortedAwake;

        private string _assetPath;

        // private int _orderSet = -1;
        private readonly List<string> _errorScriptNames = new();

        private void OnEnable() {
            this._sortedAwake = (SortedAwake)this.target;
            var monoScript = MonoScript.FromMonoBehaviour(this._sortedAwake);
            this._assetPath = AssetDatabase.GetAssetPath(monoScript);
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            // var monoImporter = (MonoImporter)AssetImporter.GetAtPath(this._assetPath);
            // var executionOrder = MonoImporter.GetExecutionOrder(monoImporter.GetScript());
            // if (executionOrder == 0) {
            //     EditorGUILayout.HelpBox("脚本执行顺序未设置, 请设置!", MessageType.Error);
            // }
            //
            // this._errorScriptNames.Clear();
            // for (int i = 0, len = this._sortedAwake.transform.childCount; i < len; i++) {
            //     var child = this._sortedAwake.transform.GetChild(i);
            //     if (child.GetComponent<SortedAwakeIgnore>()) continue;
            //     
            //     foreach (var component in child.GetComponentsInChildren<MonoBehaviour>()) {
            //         var childMonoScript = MonoScript.FromMonoBehaviour(component);
            //         var childAssetPath = AssetDatabase.GetAssetPath(childMonoScript);
            //         var childMonoImporter = (MonoImporter)AssetImporter.GetAtPath(childAssetPath);
            //         var childExecutionOrder = MonoImporter.GetExecutionOrder(childMonoImporter.GetScript());
            //         if (childExecutionOrder <= executionOrder) {
            //             if (!this._errorScriptNames.Contains(childMonoScript.name)) {
            //                 this._errorScriptNames.Add(childMonoScript.name);    
            //             }
            //         }
            //     }
            // }
            //
            // if (this._errorScriptNames.Count != 0) {
            //     EditorGUILayout.HelpBox("以下脚本的顺序执行在SortedAwake之前!", MessageType.Error);
            //     EditorGUI.indentLevel++;
            //     foreach (var scriptName in this._errorScriptNames) {
            //         EditorGUILayout.TextField(scriptName);
            //     }
            //     EditorGUI.indentLevel--;
            // }
            //
            // EditorGUI.BeginDisabledGroup(true);
            // EditorGUILayout.IntField("当前顺序", executionOrder);
            // EditorGUI.EndDisabledGroup();
            // EditorGUILayout.HelpBox("建议-1即可, 不要超到系统本身的脚本前面, 保证在子物体前就行", MessageType.Info);
            // EditorGUILayout.BeginHorizontal();
            // this._orderSet = EditorGUILayout.IntField("目标顺序", this._orderSet);
            // if (GUILayout.Button("设置为目标顺序")) {
            //     monoImporter = (MonoImporter)AssetImporter.GetAtPath(this._assetPath);
            //     MonoImporter.SetExecutionOrder(monoImporter.GetScript(), this._orderSet);
            // }
            // EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("打开窗口")) {
                // EditorApplication.ExecuteMenuItem("Edit/Project Settings/Script Execution Order"); // 这个是用来执行菜单的功能的, 但如果菜单选项是打开窗口类的, 则会报错

                // 代码打开编辑器的项目设置窗口, 并定位到脚本执行顺序的分页
                var windowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ProjectSettingsWindow");
                var editorWindow = EditorWindow.GetWindow(windowType);
                editorWindow.Show();
                // 这是个内部方法, 所以要通过反射调用
                var method = windowType.GetMethod("SelectProviderByName", AssemblyHelper.BindingFlagsInstanceIgnorePublic);
                // 该函数的name参数其实是一个局部路径名
                method?.Invoke(editorWindow, new object[] { "Project/Script Execution Order" });
            }
        }
    }

#endif
}
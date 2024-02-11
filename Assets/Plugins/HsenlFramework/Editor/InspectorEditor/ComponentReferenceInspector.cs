// using Sirenix.OdinInspector.Editor;
// using UnityEditor;
// using UnityEngine;
//
// namespace Hsenl {
//     // 实现类似unity那种点击脚本名可以在资源管理器中锁定该脚本的功能
//     [CustomEditor(typeof(ComponentReference))]
//     public class ComponentReferenceInspector : OdinEditor {
//         protected SerializedObject serializedTarget;
//         protected UnityEngine.Object           monoScript;
//         
//         protected override void OnEnable()
//         {
//             this.serializedTarget = new SerializedObject(this.target);
//             this.monoScript       = MonoScript.FromMonoBehaviour(this.target as MonoBehaviour);
//         }
//         
//         public override void OnInspectorGUI() {
//             base.OnInspectorGUI();
//         }
//         
//         protected void DrawMonoScript()
//         {
//             EditorGUI.BeginDisabledGroup(true);
//             EditorGUILayout.ObjectField("Script", this.monoScript, typeof(MonoScript), false);
//             EditorGUI.EndDisabledGroup();
//         }
//     }
// }
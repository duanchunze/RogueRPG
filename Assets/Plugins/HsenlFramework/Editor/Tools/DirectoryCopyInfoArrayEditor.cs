using UnityEngine;
using UnityEditor;

namespace Hsenl {
    // [CustomPropertyDrawer(typeof(IOCopyEditor.DirectoryCopyInfo))]
    // public class DirectoryCopyInfoArrayEditor : PropertyDrawer {
    //     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    //         EditorGUI.BeginProperty(position, label, property);
    //
    //         // 获取数组属性
    //         var arrayProp = property.FindPropertyRelative("array");
    //
    //         // 计算行数和单行高度
    //         int arraySize = arrayProp.arraySize;
    //         float singleLineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
    //         float totalHeight = singleLineHeight * (arraySize + 1); // 包括一个添加按钮的空间
    //
    //         // 创建垂直滚动区域并计算其矩形
    //         var contentRect = new Rect(position.x, position.y, position.width, totalHeight);
    //         position.height = totalHeight;
    //
    //         EditorGUI.BeginChangeCheck();
    //         EditorGUI.PropertyField(contentRect, property, true);
    //
    //         // 显示并编辑数组元素
    //         EditorGUI.indentLevel++;
    //         for (int i = 0; i < arraySize; i++) {
    //             var element = arrayProp.GetArrayElementAtIndex(i);
    //             EditorGUILayout.PropertyField(element, new GUIContent($"Element {i}"), GUILayout.MinWidth(100f));
    //         }
    //
    //         EditorGUI.indentLevel--;
    //
    //         // 添加删除和增加数组元素的功能
    //         if (GUILayout.Button("Add Element")) {
    //             arrayProp.arraySize++;
    //         }
    //
    //         if (EditorGUI.EndChangeCheck()) {
    //             property.serializedObject.ApplyModifiedProperties();
    //         }
    //
    //         EditorGUI.EndProperty();
    //     }
    // }
}
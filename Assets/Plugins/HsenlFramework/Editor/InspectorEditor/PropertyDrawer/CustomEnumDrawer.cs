// using System;
// using System.Collections.Generic;
// using UnityEditor;
// using UnityEngine;
//
// namespace Hsenl {
//     // 自定义枚举的显示方式
//     // 使用举例
//     // public enum TestEnum {
//     //      [Header("一")] // 自定义枚举的显示名
//     //      one,
//     //      [Header("二")]
//     //      two,
//     // }
//     // public class Test{
//     //      [CustomEnumLabel("xxx")] // 这里必须要添加该属性
//     //      public TestEnum te;
//     // }
//     
//     [CustomPropertyDrawer(typeof(CustomEnumLabelAttribute))]
//     public class CustomEnumDrawer : PropertyDrawer {
//         private readonly List<string> _displayNames = new();
//
//         public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
//             var celAtt = (CustomEnumLabelAttribute)this.attribute;
//             EditorHelper.GetFieldValueOfPath(property.serializedObject.targetObject, property.propertyPath, out var field);
//             var enumType = field.FieldType;
//             this._displayNames.Clear();
//             foreach (var enumName in property.enumNames) {
//                 var enumField = enumType.GetField(enumName);
//                 var hds = enumField.GetCustomAttributes(typeof(HeaderAttribute), false);
//                 this._displayNames.Add(hds.Length <= 0 ? enumName : ((HeaderAttribute)hds[0]).header);
//             }
//
//             EditorGUI.BeginChangeCheck();
//             var displayName = string.IsNullOrEmpty(celAtt.displayName) ? property.displayName : celAtt.displayName;
//             var value = EditorGUI.Popup(position, displayName, property.enumValueIndex, this._displayNames.ToArray());
//             if (EditorGUI.EndChangeCheck()) {
//                 property.enumValueIndex = value;
//             }
//         }
//     }
// }
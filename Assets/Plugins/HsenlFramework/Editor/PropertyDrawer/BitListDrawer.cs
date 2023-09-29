using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Hsenl {
    #region unity 版本

    [CustomPropertyDrawer(typeof(Bitlist))]
    public class BitListDrawer : PropertyDrawer {
        private readonly StringBuilder _builder = new();
        private readonly StringBuilder _searchBuilder = new(); // 搜索内容
        private readonly BidirectionalDictionary<string, int> _enumDict = new();
        private readonly List<(string name, int index)> _searchFilteredEnums = new(); // 被搜索过滤后的内容

        private int _inputNum; // 输入的页编号
        private string _searchContent; // 搜索的内容
        private bool _foldout; // 是否折叠
        private int _pageTotal; // 总共多少页
        private int _pageIndex; // 页编号
        private int _pageEachCount = 7; // 每页最多显示的数量
        private bool _infinitePageEach; // 打开后, 将不在分页显示
        private int PageEachCount => this._infinitePageEach ? int.MaxValue : this._pageEachCount;

        // private string PrefsFoldout => "BitListDrawer_Foldout_1";

        public BitListDrawer() {
            // this._foldout = EditorPrefs.GetBool(PrefsFoldout, false);
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            // 获取BitList的实例，再获取其中的bits数据，重写显示逻辑
            // 但无法直接获取BitList的实例，因为其作为一个字段，它可能是空的
            // 所以要先获取字段所属mono对象的实例，再获取...

            var targetObject = property.serializedObject.targetObject; // 字段所在的那个实例对象
            if (EditorHelper.GetFieldValueOfPath(targetObject, property.propertyPath, out var field) is not Bitlist bitList) { // 尝试获取BitList的实例
                // 如果为空，则使用默认绘制
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            // 在对象修改前, 记录对象当前数据, 用于支持回撤
            Undo.RecordObject(targetObject, "bit_list_operation_record" + targetObject.name);

            EditorGUI.BeginProperty(position, label, property); // 加不加没啥变化好像

            BitListShowOfEnumAttribute attr = null;
            foreach (var obj in field.GetCustomAttributes(true)) {
                if (obj is not BitListShowOfEnumAttribute a) continue;
                attr = a;
                break;
            }

            if (attr == null) {
                var list = bitList.ToList();
                this._builder.Clear();
                foreach (var i in list) {
                    this._builder.Append(i + ",");
                }

                EditorGUI.PrefixLabel(position, label);

                var indentLevel = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                // var indentOffset = EditorGUI.indentLevel * 15f; // 缩放偏移宽度
                var totalWidth = position.width; // 总宽
                var labelWidth = EditorGUIUtility.labelWidth; // 标签的宽度
                var contentWidth = totalWidth - labelWidth; // 内容宽度
                var leftmostX = position.x; // 最左边x
                var rightmostX = leftmostX + totalWidth; // 最右边x
                var contentX = leftmostX + labelWidth; // 内容的 x 起点

                position.height = EditorGUIUtility.singleLineHeight;
                position.width = contentWidth;

                position.x = contentX;
                position.width *= 0.5f;
                this._inputNum = EditorGUI.IntField(position, this._inputNum);

                position.x += position.width + 2;
                position.width *= 0.5f;
                if (GUI.Button(position, new GUIContent("添加"))) {
                    bitList.Add(this._inputNum);
                }

                position.x += position.width;
                if (GUI.Button(position, new GUIContent("删除"))) {
                    bitList.Remove(this._inputNum);
                }

                ExpandLine(ref position);

                position.width = contentWidth;
                position.x = contentX;
                EditorGUI.TextField(position, this._builder.ToString());

                EditorGUI.indentLevel = indentLevel;
            }
            else {
                // - 得到该枚举里所有的元素
                var enumType = attr.enumType;
                if (!enumType.IsEnum) throw new Exception($"arg must be a enum type! {targetObject.name} - {property.name}");
                this._enumDict.Clear();
                foreach (var enumName in Enum.GetNames(enumType)) {
                    var enumField = enumType.GetField(enumName);
                    var finalName = enumName;
                    foreach (var at in enumField.GetCustomAttributes(false)) {
                        if (at is HeaderAttribute headerAttribute) {
                            finalName = headerAttribute.header;
                            break;
                        }

                        if (at is LabelTextAttribute labelTextAttribute) {
                            finalName = labelTextAttribute.Text;
                            break;
                        }
                    }

                    var idx = (int)enumField.GetValue(null);
                    this._enumDict.Add(finalName, idx);
                }

                // - 补充BitList里有，但枚举里没有对应值的数字
                var list = bitList.ToList();
                var exist = false;
                foreach (var num in list) {
                    if (!this._enumDict.ContainsValue(num)) {
                        exist = true;
                        this._enumDict.Add(num.ToString(), num);
                    }
                }

                // - 进行搜索过滤
                this._searchFilteredEnums.Clear();
                if (!string.IsNullOrEmpty(this._searchContent)) {
                    this._searchBuilder.Clear();
                    this._searchBuilder.Append('^');
                    for (int i = 0; i < this._searchContent.Length; i++) {
                        var chr = this._searchContent[i];
                        this._searchBuilder.Append(chr);
                    }

                    var pattern = this._searchBuilder.ToString();
                    this._enumDict.ForEach((s, i) => {
                        if (Regex.IsMatch(s, pattern, RegexOptions.IgnoreCase)) {
                            this._searchFilteredEnums.Add((s, i));
                        }
                    });
                }
                else {
                    this._enumDict.ForEach((s, i) => { this._searchFilteredEnums.Add((s, i)); });
                }

                // - 算出分页所需信息
                var filterEnumsCount = this._searchFilteredEnums.Count;
                this._pageTotal = filterEnumsCount / this.PageEachCount;

                // --------------- 上面是准备数据，下面是绘制数据

                if (exist) {
                    ExpandLine(ref position, 30);
                    EditorGUI.HelpBox(position, "存在超出枚举范围的元素，建议在枚举中添加该编号", MessageType.Warning);
                }

                // 绘制属性名
                EditorGUI.PrefixLabel(position, label); // 先绘制统一的前缀标签，返回一个修改过的坐标

                // 不进行缩放, 直接设置为0, 提前缓存, 离开时还原
                var indentLevel = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                // var indentOffset = EditorGUI.indentLevel * 15f; // 缩放偏移宽度
                var totalWidth = position.width; // 总宽
                var labelWidth = EditorGUIUtility.labelWidth; // 标签的宽度
                var contentWidth = totalWidth - labelWidth; // 内容宽度
                var leftmostX = position.x; // 最左边x
                var rightmostX = leftmostX + totalWidth; // 最右边x
                var contentX = leftmostX + labelWidth; // 内容的 x 起点

                // - 绘制折叠、左右箭头、翻页、等等按钮
                position.width = 30f;
                position.x = rightmostX - position.width;
                var content = this._infinitePageEach ? "↑" : "↓";
                if (GUI.Button(position, content)) {
                    this._infinitePageEach = !this._infinitePageEach;
                }

                position.width = 30f;
                position.x -= position.width;
                if (GUI.Button(position, new GUIContent(EditorGUIUtility.FindTexture("d_forward")))) {
                    this._pageIndex++;
                }

                position.width = 30f;
                position.x -= position.width;
                EditorGUI.LabelField(position, $"/{this._pageTotal}");

                position.width = 30f;
                position.x -= position.width;
                int.TryParse(EditorGUI.TextField(position, this._pageIndex.ToString()), out this._pageIndex);

                position.width = 30f;
                position.x -= position.width;
                if (GUI.Button(position, new GUIContent(EditorGUIUtility.FindTexture("d_back")))) {
                    this._pageIndex--;
                }

                // 对翻页进行限制
                this._pageIndex = Mathf.Clamp(this._pageIndex, 0, this._pageTotal);

                position.width = position.x - contentX;
                if (position.width < 70f) position.width = 70f;
                position.x = contentX + 10f;
                if (GUI.Button(position, $"{list.Count}/{this._enumDict.Count} Items", (GUIStyle)"dragtab")) {
                    this._foldout = !this._foldout;
                    // EditorPrefs.SetBool(PrefsFoldout, this._foldout); // 缓存折叠状态到 EditorPrefs
                }

                position.x = contentX;
                EditorGUI.BeginChangeCheck();
                this._foldout = EditorGUI.Foldout(position, this._foldout, ""); // 折叠
                if (EditorGUI.EndChangeCheck()) {
                    // EditorPrefs.SetBool(PrefsFoldout, this._foldout); // 缓存折叠状态到 EditorPrefs
                }

                if (this._foldout) {
                    // - 绘制搜索栏
                    ExpandLine(ref position);
                    position.x = contentX;
                    position.width = contentWidth - 40f;
                    this._searchContent = EditorGUI.TextField(position, this._searchContent, (GUIStyle)"ToolbarSearchTextFieldWithJump");
                    position.x = rightmostX - 30;
                    position.width = 30;
                    if (GUI.Button(position, "X")) {
                        this._searchContent = null;
                    }

                    // - 绘制分页
                    var sta = this._pageIndex * this.PageEachCount;
                    var end = sta + this.PageEachCount;
                    if (end > filterEnumsCount) end = filterEnumsCount;
                    for (var i = sta; i < end; i++) {
                        ExpandLine(ref position);

                        var (name, index) = this._searchFilteredEnums[i];
                        var contains = bitList.Contains(index);

                        position.x = contentX;
                        position.width = contentWidth - 40f;
                        if (contains) {
                            if (GUI.Button(position, name, (GUIStyle)"button")) {
                                bitList.Remove(index);
                            }
                        }
                        else {
                            if (GUI.Button(position, name, (GUIStyle)"textarea")) {
                                bitList.Add(index);
                            }
                        }

                        position.x = rightmostX - 30;
                        EditorGUI.BeginChangeCheck();
                        var toggle = EditorGUI.Toggle(position, contains);
                        if (EditorGUI.EndChangeCheck()) {
                            if (toggle) {
                                bitList.Add(index);
                            }
                            else {
                                bitList.Remove(index);
                            }
                        }
                    }
                }

                EditorGUI.indentLevel = indentLevel;
            }

            EditorGUI.EndProperty();
        }

        // 18 为 EditorGUIUtility.singleLineHeight 的值, 也就是 unity gui 每行的默认高度
        private static void ExpandLine(ref Rect position, float height = 18) {
            EditorGUILayout.GetControlRect(false, height);
            position.y += height + 2;
            position.height = height;
        }
    }

    #endregion

    #region odin 版本

    public class BitListDrawerOdin : OdinAttributeDrawer<BitListShowOfEnumAttribute, Bitlist> {
        private readonly StringBuilder _searchBuilder = new();
        private readonly BidirectionalDictionary<string, int> _enumDict = new();
        private readonly List<(string name, int index)> _searchFilteredEnums = new();

        private int _inputNum;

        private float _height;
        private bool _calculatedHeight;
        private string _searchContent;
        private bool _foldout;
        private int _pageTotal;
        private int _pageIndex;
        private int _pageEachCount = 7;
        private bool _infinitePageEach;
        private int PageEachCount => this._infinitePageEach ? int.MaxValue : this._pageEachCount;

        protected override void DrawPropertyLayout(GUIContent label) {
            var position = EditorGUILayout.GetControlRect();
            var bitList = this.ValueEntry.SmartValue;
            // - 得到该枚举里所有的元素
            var enumType = this.Attribute.enumType;
            if (!enumType.IsEnum) throw new Exception($"arg must be a enum type! {this.Property} - {this.Property.Name}");
            this._enumDict.Clear();
            foreach (var enumName in Enum.GetNames(enumType)) {
                var enumField = enumType.GetField(enumName);
                var finalName = enumName;
                foreach (var at in enumField.GetCustomAttributes(false)) {
                    if (at is HeaderAttribute headerAttribute) {
                        finalName = headerAttribute.header;
                        break;
                    }

                    if (at is LabelTextAttribute labelTextAttribute) {
                        finalName = labelTextAttribute.Text;
                        break;
                    }
                }

                var idx = (int)enumField.GetValue(null);
                this._enumDict.Add(finalName, idx);
            }

            // - 补充BitList里有，但枚举里没有对应值的数字
            var list = bitList.ToList();
            var exist = false;
            foreach (var num in list) {
                if (!this._enumDict.ContainsValue(num)) {
                    exist = true;
                    this._enumDict.Add(num.ToString(), num);
                }
            }

            // - 进行搜索过滤
            this._searchFilteredEnums.Clear();
            if (!string.IsNullOrEmpty(this._searchContent)) {
                this._searchBuilder.Clear();
                this._searchBuilder.Append('^');
                for (int i = 0; i < this._searchContent.Length; i++) {
                    var chr = this._searchContent[i];
                    this._searchBuilder.Append(chr);
                }

                var pattern = this._searchBuilder.ToString();
                this._enumDict.ForEach((s, i) => {
                    if (Regex.IsMatch(s, pattern, RegexOptions.IgnoreCase)) {
                        this._searchFilteredEnums.Add((s, i));
                    }
                });
            }
            else {
                this._enumDict.ForEach((s, i) => { this._searchFilteredEnums.Add((s, i)); });
            }

            // - 算出分页所需信息
            var filterEnumsCount = this._searchFilteredEnums.Count;
            this._pageTotal = filterEnumsCount / this.PageEachCount;

            // --------------- 上面是准备数据，下面是绘制数据

            if (exist) {
                ExpandLine(ref position, 30);
                EditorGUI.HelpBox(position, "存在超出枚举范围的元素，建议在枚举中添加该编号", MessageType.Warning);
            }

            // 绘制属性名
            if (label != null) {
                EditorGUI.PrefixLabel(position, label); // 先绘制统一的前缀标签，返回一个修改过的坐标    
            }

            // 不进行缩放, 直接设置为0, 提前缓存, 离开时还原
            var indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // var indentOffset = EditorGUI.indentLevel * 15f; // 缩放偏移宽度
            var totalWidth = position.width; // 总宽
            var labelWidth = EditorGUIUtility.labelWidth; // 标签的宽度
            var contentWidth = totalWidth - labelWidth; // 内容宽度
            var leftmostX = position.x; // 最左边x
            var rightmostX = leftmostX + totalWidth; // 最右边x
            var contentX = leftmostX + labelWidth; // 内容的 x 起点

            // - 绘制折叠、左右箭头、翻页、等等按钮
            position.width = 30f;
            position.x = rightmostX - position.width;
            var content = this._infinitePageEach ? "↑" : "↓";
            if (GUI.Button(position, content)) {
                this._infinitePageEach = !this._infinitePageEach;
            }

            position.width = 30f;
            position.x -= position.width;
            if (GUI.Button(position, new GUIContent(EditorGUIUtility.FindTexture("d_forward")))) {
                this._pageIndex++;
            }

            position.width = 30f;
            position.x -= position.width;
            EditorGUI.LabelField(position, $"/{this._pageTotal}");

            position.width = 30f;
            position.x -= position.width;
            int.TryParse(EditorGUI.TextField(position, this._pageIndex.ToString()), out this._pageIndex);

            position.width = 30f;
            position.x -= position.width;
            if (GUI.Button(position, new GUIContent(EditorGUIUtility.FindTexture("d_back")))) {
                this._pageIndex--;
            }

            // 对翻页进行限制
            this._pageIndex = Mathf.Clamp(this._pageIndex, 0, this._pageTotal);

            position.width = position.x - contentX;
            if (position.width < 70f) position.width = 70f;
            position.x = contentX + 10f;
            if (GUI.Button(position, $"{list.Count}/{this._enumDict.Count} Items", (GUIStyle)"dragtab")) {
                this._foldout = !this._foldout;
                // EditorPrefs.SetBool(PrefsFoldout, this._foldout); // 缓存折叠状态到 EditorPrefs
            }

            position.x = contentX;
            EditorGUI.BeginChangeCheck();
            this._foldout = EditorGUI.Foldout(position, this._foldout, ""); // 折叠
            if (EditorGUI.EndChangeCheck()) {
                // EditorPrefs.SetBool(PrefsFoldout, this._foldout); // 缓存折叠状态到 EditorPrefs
            }

            if (this._foldout) {
                // - 绘制搜索栏
                ExpandLine(ref position);
                position.x = contentX;
                position.width = contentWidth - 40f;
                this._searchContent = EditorGUI.TextField(position, this._searchContent, (GUIStyle)"ToolbarSearchTextFieldWithJump");
                position.x = rightmostX - 30;
                position.width = 30;
                if (GUI.Button(position, "X")) {
                    this._searchContent = null;
                }

                // - 绘制分页
                var sta = this._pageIndex * this.PageEachCount;
                var end = sta + this.PageEachCount;
                if (end > filterEnumsCount) end = filterEnumsCount;
                for (var i = sta; i < end; i++) {
                    ExpandLine(ref position);

                    var (name, index) = this._searchFilteredEnums[i];
                    var contains = bitList.Contains(index);

                    position.x = contentX;
                    position.width = contentWidth - 40f;
                    if (contains) {
                        if (GUI.Button(position, name, (GUIStyle)"button")) {
                            bitList.Remove(index);
                        }
                    }
                    else {
                        if (GUI.Button(position, name, (GUIStyle)"textarea")) {
                            bitList.Add(index);
                        }
                    }

                    position.x = rightmostX - 30;
                    EditorGUI.BeginChangeCheck();
                    var toggle = EditorGUI.Toggle(position, contains);
                    if (EditorGUI.EndChangeCheck()) {
                        if (toggle) {
                            bitList.Add(index);
                        }
                        else {
                            bitList.Remove(index);
                        }
                    }
                }
            }

            EditorGUI.indentLevel = indentLevel;
        }

        private static void ExpandLine(ref Rect position, float height = 18) {
            EditorGUILayout.GetControlRect(false, height);
            position.y += height + 2;
            position.height = height;
        }
    }

    #endregion
}
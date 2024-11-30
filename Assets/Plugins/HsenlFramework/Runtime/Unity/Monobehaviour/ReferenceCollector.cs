/*MIT License

Copyright (c) 2018 tanghai

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections.Generic;
using UnityEngine;
//Object并非C#基础中的Object，而是 UnityEngine.Object
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

//使其能在Inspector面板显示，并且可以被赋予相应值
[Serializable]
public class ReferenceCollectorData {
    public string key;

    //Object并非C#基础中的Object，而是 UnityEngine.Object
    public Object gameObject;
}

//继承IComparer对比器，Ordinal会使用序号排序规则比较字符串，因为是byte级别的比较，所以准确性和性能都不错
public class ReferenceCollectorDataComparer : IComparer<ReferenceCollectorData> {
    public int Compare(ReferenceCollectorData x, ReferenceCollectorData y) {
        return string.Compare(x.key, y.key, StringComparison.Ordinal);
    }
}

//继承ISerializationCallbackReceiver后会增加OnAfterDeserialize和OnBeforeSerialize两个回调函数，如果有需要可以在对需要序列化的东西进行操作
//ET在这里主要是在OnAfterDeserialize回调函数中将data中存储的ReferenceCollectorData转换为dict中的Object，方便之后的使用
//注意UNITY_EDITOR宏定义，在编译以后，部分编辑器相关函数并不存在
public class ReferenceCollector : MonoBehaviour, ISerializationCallbackReceiver {
    //用于序列化的List
    public List<ReferenceCollectorData> data = new List<ReferenceCollectorData>();

    //Object并非C#基础中的Object，而是 UnityEngine.Object
    private readonly Dictionary<string, Object> dict = new Dictionary<string, Object>();

#if UNITY_EDITOR
    //添加新的元素
    public void Add(string key, Object obj) {
        UnityEditor.SerializedObject serializedObject = new UnityEditor.SerializedObject(this);
        //根据PropertyPath读取数据
        //如果不知道具体的格式，可以右键用文本编辑器打开一个prefab文件（如Bundles/UI目录中的几个）
        //因为这几个prefab挂载了ReferenceCollector，所以搜索data就能找到存储的数据
        UnityEditor.SerializedProperty dataProperty = serializedObject.FindProperty("data");
        int i;
        //遍历data，看添加的数据是否存在相同key
        for (i = 0; i < this.data.Count; i++) {
            if (this.data[i].key == key) {
                break;
            }
        }

        //不等于data.Count意为已经存在于data List中，直接赋值即可
        if (i != this.data.Count) {
            //根据i的值获取dataProperty，也就是data中的对应ReferenceCollectorData，不过在这里，是对Property进行的读取，有点类似json或者xml的节点
            UnityEditor.SerializedProperty element = dataProperty.GetArrayElementAtIndex(i);
            //对对应节点进行赋值，值为gameobject相对应的fileID
            //fileID独一无二，单对单关系，其他挂载在这个gameobject上的script或组件会保存相对应的fileID
            element.FindPropertyRelative("gameObject").objectReferenceValue = obj;
        }
        else {
            //等于则说明key在data中无对应元素，所以得向其插入新的元素
            dataProperty.InsertArrayElementAtIndex(i);
            UnityEditor.SerializedProperty element = dataProperty.GetArrayElementAtIndex(i);
            element.FindPropertyRelative("key").stringValue = key;
            element.FindPropertyRelative("gameObject").objectReferenceValue = obj;
        }

        //应用与更新
        UnityEditor.EditorUtility.SetDirty(this);
        serializedObject.ApplyModifiedProperties();
        serializedObject.UpdateIfRequiredOrScript();
    }

    //删除元素，知识点与上面的添加相似
    public void Remove(string key) {
        UnityEditor.SerializedObject serializedObject = new UnityEditor.SerializedObject(this);
        UnityEditor.SerializedProperty dataProperty = serializedObject.FindProperty("data");
        int i;
        for (i = 0; i < this.data.Count; i++) {
            if (this.data[i].key == key) {
                break;
            }
        }

        if (i != this.data.Count) {
            dataProperty.DeleteArrayElementAtIndex(i);
        }

        UnityEditor.EditorUtility.SetDirty(this);
        serializedObject.ApplyModifiedProperties();
        serializedObject.UpdateIfRequiredOrScript();
    }

    public void Clear() {
        UnityEditor.SerializedObject serializedObject = new UnityEditor.SerializedObject(this);
        //根据PropertyPath读取prefab文件中的数据
        //如果不知道具体的格式，可以直接右键用文本编辑器打开，搜索data就能找到
        var dataProperty = serializedObject.FindProperty("data");
        dataProperty.ClearArray();
        UnityEditor.EditorUtility.SetDirty(this);
        serializedObject.ApplyModifiedProperties();
        serializedObject.UpdateIfRequiredOrScript();
    }

    public void Sort() {
        UnityEditor.SerializedObject serializedObject = new UnityEditor.SerializedObject(this);
        this.data.Sort(new ReferenceCollectorDataComparer());
        UnityEditor.EditorUtility.SetDirty(this);
        serializedObject.ApplyModifiedProperties();
        serializedObject.UpdateIfRequiredOrScript();
    }

    public void UpdateDatas() {
        var list = new List<Object>();
        foreach (var v in this.data) {
            list.Add(v.gameObject);
        }

        this.Clear();

        foreach (var o in list) {
            this.Add(o.name, o);
        }
    }

#endif

    public T Get<T>() where T : class {
        return this.Get<T>(typeof(T).Name);
    }

    //使用泛型返回对应key的gameobject
    public T Get<T>(string key) where T : class {
        if (!this.dict.TryGetValue(key, out Object obj)) {
            return null;
        }

        if (obj is GameObject go && typeof(T) != typeof(GameObject)) {
            return go.GetComponent<T>();
        }

        return obj as T;
    }

    public IReadOnlyDictionary<string, Object> GetAll() {
        return this.dict;
    }

    public Object GetObject(string key) {
        if (!this.dict.TryGetValue(key, out Object dictGo)) {
            return null;
        }

        return dictGo;
    }

    public void OnBeforeSerialize() { }

    //在反序列化后运行
    public void OnAfterDeserialize() {
        this.dict.Clear();
        foreach (ReferenceCollectorData referenceCollectorData in this.data) {
            if (!this.dict.ContainsKey(referenceCollectorData.key)) {
                this.dict.Add(referenceCollectorData.key, referenceCollectorData.gameObject);
            }
        }
    }
}

#if UNITY_EDITOR

//自定义ReferenceCollector类在界面中的显示与功能
[CustomEditor(typeof(ReferenceCollector))]
public class ReferenceCollectorEditor : Editor {
    //输入在textfield中的字符串
    private string searchKey {
        get { return this._searchKey; }
        set {
            if (this._searchKey != value) {
                this._searchKey = value;
                this.heroPrefab = this.referenceCollector.Get<Object>(this.searchKey);
            }
        }
    }

    private ReferenceCollector referenceCollector;

    private Object heroPrefab;

    private string _searchKey = "";

    private void DelNullReference() {
        SerializedProperty dataProperty = this.serializedObject.FindProperty("data");
        for (int i = dataProperty.arraySize - 1; i >= 0; i--) {
            SerializedProperty gameObjectProperty = dataProperty.GetArrayElementAtIndex(i).FindPropertyRelative("gameObject");
            if (gameObjectProperty.objectReferenceValue == null) {
                dataProperty.DeleteArrayElementAtIndex(i);
                EditorUtility.SetDirty(this.referenceCollector);
                this.serializedObject.ApplyModifiedProperties();
                this.serializedObject.UpdateIfRequiredOrScript();
            }
        }
    }

    private void OnEnable() {
        //将被选中的gameobject所挂载的ReferenceCollector赋值给编辑器类中的ReferenceCollector，方便操作
        this.referenceCollector = (ReferenceCollector)this.target;
    }

    public override void OnInspectorGUI() {
        //使ReferenceCollector支持撤销操作，还有Redo，不过没有在这里使用
        Undo.RecordObject(this.referenceCollector, "Changed Settings");
        SerializedProperty dataProperty = this.serializedObject.FindProperty("data");
        //开始水平布局，如果是比较新版本学习U3D的，可能不知道这东西，这个是老GUI系统的知识，除了用在编辑器里，还可以用在生成的游戏中
        GUILayout.BeginHorizontal();
        //下面几个if都是点击按钮就会返回true调用里面的东西
        // if (GUILayout.Button("添加引用")) { // 用不到
        //     //添加新的元素，具体的函数注释
        //     // Guid.NewGuid().GetHashCode().ToString() 就是新建后默认的key // 用不到
        //     this.AddReference(dataProperty, Guid.NewGuid().GetHashCode().ToString(), null); // 用不到
        // }

        if (GUILayout.Button("全部删除")) {
            this.referenceCollector.Clear();
        }

        if (GUILayout.Button("删除空引用")) {
            this.DelNullReference();
        }

        if (GUILayout.Button("排序")) {
            this.referenceCollector.Sort();
        }
        
        if (GUILayout.Button("更新")) {
            this.referenceCollector.UpdateDatas();
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        //可以在编辑器中对searchKey进行赋值，只要输入对应的Key值，就可以点后面的删除按钮删除相对应的元素
        this.searchKey = EditorGUILayout.TextField(this.searchKey);
        //添加的可以用于选中Object的框，这里的object也是(UnityEngine.Object
        //第三个参数为是否只能引用scene中的Object
        // EditorGUILayout.ObjectField(this.heroPrefab, typeof(Object), false); // 用不到
        // if (GUILayout.Button("删除", GUILayout.MaxWidth(60))) { // 用不到
        //     this.referenceCollector.Remove(this.searchKey); // 用不到
        //     this.heroPrefab = null; // 用不到
        // }

        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        List<int> delList = new();
        //遍历ReferenceCollector中data list的所有元素，显示在编辑器中
        for (int i = this.referenceCollector.data.Count - 1; i >= 0; i--) {
            GUILayout.BeginHorizontal();
            //这里的知识点在ReferenceCollector中有说
            SerializedProperty property = dataProperty.GetArrayElementAtIndex(i).FindPropertyRelative("key");
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField(property.stringValue, GUILayout.Width(150));
            EditorGUI.EndDisabledGroup();
            property = dataProperty.GetArrayElementAtIndex(i).FindPropertyRelative("gameObject");
            property.objectReferenceValue = EditorGUILayout.ObjectField(property.objectReferenceValue, typeof(Object), true);
            if (GUILayout.Button("X")) {
                //将元素添加进删除list
                delList.Add(i);
            }

            GUILayout.EndHorizontal();
        }

        EventType eventType = Event.current.type;
        //在Inspector 窗口上创建区域，向区域拖拽资源对象，获取到拖拽到区域的对象
        if (eventType == EventType.DragUpdated || eventType == EventType.DragPerform) {
            // Show a copy icon on the drag
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (eventType == EventType.DragPerform) {
                DragAndDrop.AcceptDrag();
                foreach (Object o in DragAndDrop.objectReferences) {
                    this.AddReference(dataProperty, o.name, o);
                }
            }

            Event.current.Use();
        }

        //遍历删除list，将其删除掉
        foreach (int i in delList) {
            dataProperty.DeleteArrayElementAtIndex(i);
        }

        this.serializedObject.ApplyModifiedProperties();
        this.serializedObject.UpdateIfRequiredOrScript();
    }

    //添加元素，具体知识点在ReferenceCollector中说了
    private void AddReference(SerializedProperty dataProperty, string key, Object obj) {
        int index = dataProperty.arraySize;
        dataProperty.InsertArrayElementAtIndex(index);
        SerializedProperty element = dataProperty.GetArrayElementAtIndex(index);
        element.FindPropertyRelative("key").stringValue = key;
        element.FindPropertyRelative("gameObject").objectReferenceValue = obj;
    }
}

#endif
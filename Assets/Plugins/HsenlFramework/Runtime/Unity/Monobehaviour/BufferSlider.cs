using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BufferSlider : MonoBehaviour {
    [SerializeField] private Slider _mainSlider;
    [SerializeField] private Slider _bufferSlider;
    [SerializeField] private Vector2 _bufferSpeed = new(0.25f, 3.5f);

    private void OnEnable() {
        this._bufferSlider.value = this._mainSlider.value;
    }

    private void Update() {
        if (this._mainSlider && this._bufferSlider) {
            if (this._mainSlider.value < this._bufferSlider.value) {
                // 进度条差值越大，缓冲的越快
                var sub = Time.deltaTime * Mathf.Lerp(this._bufferSpeed.x, this._bufferSpeed.y, this._bufferSlider.value - this._mainSlider.value);
                this._bufferSlider.value -= sub;
            }
            else {
                this._bufferSlider.value = this._mainSlider.value;
            }
        }
    }
    
    #if UNITY_EDITOR

    [CustomEditor(typeof(BufferSlider))]
    public class BufferSliderEditor : Editor {
        private BufferSlider _t;
        private UnityEngine.Object _monoScript;

        private SerializedProperty _mainSlider;
        private SerializedProperty _bufferSlider;
        private SerializedProperty _bufferSpeed;

        protected void OnEnable() {
            this._t = (BufferSlider)this.target;
            this._monoScript = MonoScript.FromMonoBehaviour((MonoBehaviour)this.target);
            
            this._mainSlider = this.serializedObject.FindProperty("_mainSlider");
            this._bufferSlider = this.serializedObject.FindProperty("_bufferSlider");
            this._bufferSpeed = this.serializedObject.FindProperty("_bufferSpeed");

            if (this._mainSlider.objectReferenceValue == null)
                this._mainSlider.objectReferenceValue = this._t.transform.parent.GetComponentInParent<Slider>();

            if (this._bufferSlider.objectReferenceValue == null)
                this._bufferSlider.objectReferenceValue = this._t.GetComponent<Slider>();

            this.serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI() {
            this.DrawMonoScript();

            EditorGUILayout.PropertyField(this._mainSlider);
            EditorGUILayout.PropertyField(this._bufferSlider);
            EditorGUILayout.PropertyField(this._bufferSpeed);

            this.serializedObject.ApplyModifiedProperties();
        }

        private void DrawMonoScript() {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", this._monoScript, typeof(MonoScript), false);
            EditorGUI.EndDisabledGroup();
        }
    }

#endif
}
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class VolumeSlider : MonoBehaviour {
    public enum Model {
        Normal, // 就是普通的血条
        Valume, // 类似DNF里BOSS的那种血条
    }

    public struct SliderConfiguration {
        public readonly bool isCreated;
        public readonly bool useGradient; // 使用渐变色
        public Color color1;
        public Color color2;

        public SliderConfiguration(bool useGradient) {
            this.isCreated = true;
            this.useGradient = useGradient;
            this.color1 = default;
            this.color2 = default;
        }
    }

    [SerializeField]
    private Model _sliderModel;

    [SerializeField]
    private Slider _slider;

    [SerializeField]
    private Image _fillImageOfNextLine;

    [SerializeField]
    private Image _fillImage;

    [SerializeField]
    private int _totalVolume; // 总共的最大值（比如该Boss一共有5000血量）

    [SerializeField]
    private int _firstLineVolume; // 第一行值（比如第一行分了1000）

    [SerializeField]
    private int _incremental; // 增量（后一行比前一行增加多少，比如增加100，但一般增量都设置为0就行）, 那么最后血条就是1000、1100、1200、1300、400，共5行血

    [SerializeField]
    private List<Sprite> _lineSprites = new();

    private readonly List<float> _lineVolumes = new();
    private readonly List<float> _lineInfos = new();

    private void Awake() {
        if (Application.isPlaying) {
            if (this._lineSprites.Count == 0) {
                this._lineSprites.Add(this._fillImage.sprite);
            }

            if (this._sliderModel == Model.Valume) {
                this.Initialize();
            }
        }
    }

    public void Initialize() {
        if (this._lineVolumes.Count == 0) {
            if (this._firstLineVolume == 0) {
                Debug.LogError("val slider first val cant be '0'!");
                return;
            }

            if (this._totalVolume < this._firstLineVolume) this._totalVolume = this._firstLineVolume;
            var tmpVal = this._firstLineVolume;
            var tmpMaxVal = this._totalVolume;

            // 循环得到每一行的信息（多少血）
            do {
                this._lineVolumes.Add(tmpVal);
                tmpMaxVal -= tmpVal;
                tmpVal += this._incremental;
            } while (tmpMaxVal > tmpVal);

            this._lineVolumes.Add(tmpVal);
        }

        // 计算每一行累加之前之后得到的值，用于内部转换索引用
        float val = 0;
        this._lineInfos.Add(val);
        for (var i = 0; i < this._lineVolumes.Count; i++) {
            val += this._lineVolumes[i];
            this._lineInfos.Add(val);
        }
    }

    public void UpdateSlider(float val) {
        switch (this._sliderModel) {
            case Model.Valume:
                var index = this.ConvertValToLineIndex(val, out var margin);
                if (index != -1) {
                    var currLineVolume = this._lineVolumes[index];
                    var preIndex = index - 1;
                    if (preIndex >= 0) {
                        this._fillImageOfNextLine.gameObject.SetActive(true);
                    }
                    else {
                        this._fillImageOfNextLine.gameObject.SetActive(false);
                    }

                    // 求余
                    var sliderValue = margin / currLineVolume;
                    this._slider.value = sliderValue;

                    // 展示行数
                    var viewIndex = index + 1;
                }
                else {
                    this._slider.value = 0;
                }

                break;

            case Model.Normal:
                this._slider.value = val;
                break;
        }
    }

    public void UpdateSlider(float val, SliderConfiguration sliderConfiguration) {
        switch (this._sliderModel) {
            case Model.Valume:
                var index = this.ConvertValToLineIndex(val, out var margin);
                if (index != -1) {
                    var currLineVolume = this._lineVolumes[index];
                    var preIndex = index - 1;
                    if (preIndex >= 0) {
                        this._fillImageOfNextLine.gameObject.SetActive(true);
                        // 颜色渐变
                        if (sliderConfiguration.isCreated) {
                            if (sliderConfiguration.useGradient) {
                                this._fillImageOfNextLine.color = Color.Lerp(sliderConfiguration.color1, sliderConfiguration.color2,
                                    (float)preIndex / this._lineVolumes.Count);
                            }
                            else {
                                this._fillImageOfNextLine.sprite = this._lineSprites[preIndex % this._lineSprites.Count];
                            }
                        }
                    }
                    else {
                        this._fillImageOfNextLine.gameObject.SetActive(false);
                    }

                    // 颜色渐变
                    if (sliderConfiguration.isCreated) {
                        if (sliderConfiguration.useGradient) {
                            this._fillImage.color = Color.Lerp(sliderConfiguration.color1, sliderConfiguration.color2,
                                (float)index / this._lineVolumes.Count);
                        }
                        else {
                            this._fillImage.sprite = this._lineSprites[index % this._lineSprites.Count];
                        }
                    }

                    // 求余
                    var result = margin / currLineVolume;
                    this._slider.value = result;

                    // 展示行数
                    var viewIndex = index + 1;
                }
                else {
                    this._slider.value = 0;
                }

                break;

            case Model.Normal:
                this._slider.value = val;

                if (sliderConfiguration.isCreated) {
                    if (sliderConfiguration.useGradient) {
                        this._fillImage.color = Color.Lerp(sliderConfiguration.color1, sliderConfiguration.color2, val);
                    }
                }

                break;
        }
    }

    /// <summary>
    /// 把值转成条索引
    /// 假设现在总共有3000血, 分了3行, 每行血分别为1000, 现在要设置为2500血, 那么返回的就是第二行, 盈余500
    /// </summary>
    /// <param name="val"></param>
    /// <param name="margin"></param>
    /// <returns></returns>
    private int ConvertValToLineIndex(float val, out float margin) {
        var index = 0;
        for (var i = 0; i < this._lineInfos.Count; i++) {
            if (this._lineInfos[i] >= val) {
                index = i - 1;
                if (i == 0) {
                    margin = val;
                }
                else {
                    margin = val - this._lineInfos[index];
                }

                return index;
            }
        }

        index = this._lineInfos.Count - 1;
        margin = val - this._lineInfos[index];
        return index;
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(VolumeSlider))]
    public class VolumeSliderEditor : Editor {
        private VolumeSlider _t;
        private UnityEngine.Object _monoScript;

        private SerializedProperty _sliderModel;
        private SerializedProperty _slider;
        private SerializedProperty _fillImageOfNextLine;
        private SerializedProperty _fillImage;
        private SerializedProperty _totalVolume;
        private SerializedProperty _firstLineVolume;
        private SerializedProperty _incremental;
        private SerializedProperty _lineSprites;

        protected void OnEnable() {
            this._t = (VolumeSlider)this.target;
            this._monoScript = MonoScript.FromMonoBehaviour((MonoBehaviour)this.target);

            this._sliderModel = this.serializedObject.FindProperty("_sliderModel");
            this._slider = this.serializedObject.FindProperty("_slider");
            this._fillImageOfNextLine = this.serializedObject.FindProperty("_fillImageOfNextLine");
            this._fillImage = this.serializedObject.FindProperty("_fillImage");
            this._totalVolume = this.serializedObject.FindProperty("_totalVolume");
            this._firstLineVolume = this.serializedObject.FindProperty("_firstLineVolume");
            this._incremental = this.serializedObject.FindProperty("_incremental");
            this._lineSprites = this.serializedObject.FindProperty("_lineSprites");

            if (this._slider.objectReferenceValue == null) {
                this._slider.objectReferenceValue = this._t.GetComponent<Slider>();
            }

            var slider = (Slider)this._slider.objectReferenceValue;
            if (this._fillImage.objectReferenceValue == null)
                this._fillImage.objectReferenceValue = slider.fillRect.GetComponent<Image>();
            if (this._fillImageOfNextLine.objectReferenceValue == null)
                this._fillImageOfNextLine.objectReferenceValue = slider.transform.Find("Background")?.GetComponent<Image>();

            this.serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI() {
            this.DrawMonoScript();

            EditorGUILayout.PropertyField(this._sliderModel);
            EditorGUILayout.PropertyField(this._slider);
            EditorGUILayout.PropertyField(this._fillImage);
            switch ((Model)this._sliderModel.enumValueFlag) {
                case Model.Normal:
                    break;
                case Model.Valume:
                    EditorGUILayout.PropertyField(this._totalVolume);
                    EditorGUILayout.PropertyField(this._firstLineVolume);
                    EditorGUILayout.PropertyField(this._incremental);
                    EditorGUILayout.PropertyField(this._fillImageOfNextLine);
                    EditorGUILayout.PropertyField(this._lineSprites);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

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
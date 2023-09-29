// using System;
// using System.Collections.Generic;
// using Sirenix.OdinInspector;
// using UnityEngine;
//
// namespace ET {
//     [Serializable]
//     public abstract class DefaultPriorityState : PriorityState {
//         protected bool handledFlag;
//         /// 注意, 该字段应该用于时间缩放, 不要用在比如技能减cd, 释放速度等乱七八糟的上面
//         [SerializeField, PropertyRange(0f, 5f), LabelText("时间缩放")] protected float timeScale = 1f;
//         // [SerializeField, LabelText("名称")] protected string name; // 不实现, 交给子类, 以组件的形式获得
//         [SerializeField, LabelText("通道")] protected List<int> aisles = new() { 0 };
//         // protected BitList labels; // 不实现, 交给子类, 以组件的形式获得
//         [SerializeField, LabelText("进入等级")] protected int enterPriority;
//         [SerializeField, LabelText("阻拦等级锚点")] protected int resistPriorityAnchor;
//         [ShowInInspector, LabelText("阻拦等级"), DisableInEditorMode] protected int resistPriority;
//         [SerializeField, LabelText("保持等级")] protected int keepPriority;
//         [SerializeField, LabelText("排挤等级")] protected int exclusionPriority;
//         [SerializeField, LabelText("运行等级")] protected int runPriority;
//         [SerializeField, LabelText("禁用等级")] protected int disablePriority;
//         [SerializeField, BitListShowOfEnum(typeof(LabelType)), LabelText("指定通过")] public BitList specialPassLabels;
//         [SerializeField, BitListShowOfEnum(typeof(LabelType)), LabelText("指定拦截")] public BitList specialInterceptLabels;
//         [SerializeField, BitListShowOfEnum(typeof(LabelType)), LabelText("指定保持")] public BitList specialKeepLabels;
//         [SerializeField, BitListShowOfEnum(typeof(LabelType)), LabelText("指定排挤")] public BitList specialExclusionLabels;
//         [SerializeField, BitListShowOfEnum(typeof(LabelType)), LabelText("指定更新")] public BitList specialRunLabels;
//         [SerializeField, BitListShowOfEnum(typeof(LabelType)), LabelText("指定禁用")] public BitList specialDisableLabels;
//
//         [ShowInInspector, ReadOnly, LabelText("上一帧暂停")] protected bool pausedPrevious;
//         [ShowInInspector, LabelText("暂停")] protected bool paused;
//         [SerializeField, LabelText("持续时间")] protected float duration;
//         [SerializeField, ReadOnly, LabelText("持续时间计时器")] protected float durationTimer;
//         [ShowInInspector, LabelText("时间暂停")] protected bool timeParse;
//         
//         [ShowInInspector, ReadOnly, LabelText("状态管理器")] private Priorities _manager;
//
//         internal override bool InternalHandledFlag {
//             get => this.handledFlag;
//             set => this.handledFlag = value;
//         }
//
//         internal override float InternalTimeScale => this.timeScale;
//
//         internal override string InternalName => this.Name;
//
//         public abstract string Name { get; }
//
//         internal override ICollection<int> InternalAisles => this.aisles;
//         
//         internal override BitList InternalLabels => this.Labels;
//
//         public abstract BitList Labels { get; }
//
//         internal override int InternalEnterPriority => this.enterPriority;
//
//         internal override int InternalResistPriorityAnchor => this.resistPriorityAnchor;
//
//         internal override int InternalResistPriority {
//             get => this.resistPriority;
//             set => this.resistPriority = value;
//         }
//
//         internal override int InternalKeepPriority => this.keepPriority;
//
//         internal override int InternalExclusionPriority => this.exclusionPriority;
//
//         internal override int InternalRunPriority => this.runPriority;
//
//         internal override int InternalDisablePriority => this.disablePriority;
//
//         internal override BitList InternalSpecialPassLabels => this.specialPassLabels;
//
//         internal override BitList InternalSpecialInterceptLabels => this.specialInterceptLabels;
//
//         internal override BitList InternalSpecialKeepLabels => this.specialKeepLabels;
//
//         internal override BitList InternalSpecialExclusionLabels => this.specialExclusionLabels;
//
//         internal override BitList InternalSpecialRunLabels => this.specialRunLabels;
//
//         internal override BitList InternalSpecialDisableLabels => this.specialDisableLabels;
//
//         internal override bool InternalPausedPrevious {
//             get => this.pausedPrevious;
//             set => this.pausedPrevious = value;
//         }
//
//         internal override bool InternalPaused {
//             get => this.paused;
//             set => this.paused = value;
//         }
//
//         internal override float InternalDurationTimer {
//             get => this.durationTimer;
//             set => this.durationTimer = value;
//         }
//
//         internal override float InternalDuration {
//             get => this.duration;
//             set => this.duration = value;
//         }
//
//         internal override bool InternalTimeParse {
//             get => this.timeParse;
//             set => this.timeParse = value;
//         }
//
//         internal override Priorities InternalManager {
//             get => this._manager;
//             set => this._manager = value;
//         }
//
//         public virtual bool IsEntered => this.InternalIsEntered;
//
//         public virtual void Dispose() {
//             this.handledFlag = false;
//             this.timeScale = 1f;
//             this._manager = null;
//             this.aisles?.Clear();
//             this.enterPriority = 0;
//             this.resistPriority = 0;
//             this.keepPriority = 0;
//             this.exclusionPriority = 0;
//             this.runPriority = 0;
//             this.disablePriority = 0;
//             this.specialPassLabels?.Clear();
//             this.specialInterceptLabels?.Clear();
//             this.specialKeepLabels?.Clear();
//             this.specialExclusionLabels?.Clear();
//             this.specialRunLabels?.Clear();
//             this.specialDisableLabels?.Clear();
//             this.pausedPrevious = false;
//             this.paused = false;
//             this.durationTimer = 0;
//             this.duration = 0;
//             this.timeScale = 1f;
//             this.timeParse = false;
//         }
//     }
// }


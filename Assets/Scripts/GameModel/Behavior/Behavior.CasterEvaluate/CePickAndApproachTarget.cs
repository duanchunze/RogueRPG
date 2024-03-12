using System;
using System.Collections.Generic;
using Hsenl.casterevaluate;
using MemoryPack;
using Unity.Mathematics;
using UnityEngine;

namespace Hsenl {
    [MemoryPackable()]
    // 自动靠近目标
    public partial class CePickAndApproachTarget : CeInfo<PickAndApproachTargetInfo> {
        private Transform _tran;
        private Control _control;
        private Selector _selector;
        private List<Numerator> _numerators = new(2);
        private Faction _faction;

        private float _distance;

        protected override void OnEnable() {
            var owner = this.manager.Bodied.AttachedBodied;
            switch (this.manager.Bodied) {
                case Ability ability: {
                    this._tran = owner?.transform;
                    this._control = owner?.GetComponent<Control>();
                    this._selector = owner?.GetComponent<Selector>();

                    this._numerators.Clear();
                    var numerator = ability.GetComponent<Numerator>();
                    if (numerator != null) this._numerators.Add(numerator);
                    numerator = owner?.GetComponent<Numerator>();
                    if (numerator != null) this._numerators.Add(numerator);

                    this._faction = owner?.GetComponent<Faction>();
                    break;
                }
            }
        }

        protected override NodeStatus OnNodeTick() {
            switch (this.manager.Bodied) {
                case Ability ability: {
                    if (this._numerators.Count == 0)
                        // 没数值器
                        break;

                    // 思路: 先尝试获得施法范围内的目标, 如果有, 则以成功跳出, 如果没有, 则尝试获取检测范围的目标, 如果也没有, 则以失败跳出, 如果有, 则向目标移动, 直到靠近目标施法范围

                    // 通过阵营获取目标的标签范围
                    var constrainsTags = this._faction?.GetTagsOfFactionTypes(ability.factionTypes);
                    // 获取作用的目标个数
                    var targetCount = GameAlgorithm.MergeCalculateNumeric(this._numerators, NumericType.Ttc);
                    // 获取施法范围
                    var castRange = GameAlgorithm.MergeCalculateNumeric(this._numerators, NumericType.Crange);
                    // 根据施法范围获取目标
                    ability.targets.Clear();
                    SelectionTarget target = null;
                    this._selector
                        .SearcherSphereBody(castRange)
                        .FilterAlive()
                        .FilterTags(constrainsTags, null)
                        .FilterObstacles()
                        .SelectNearests(targetCount)
                        .Wrap(ability.targets);

                    if (ability.targets.Count != 0)
                        target = ability.targets[0];

                    // 对目标的状况进行判断
                    if (target != null) {
                        var forward = this._tran.Forward;
                        forward.y = 0;
                        var dir = target.transform.Position - this._tran.Position;
                        dir.y = 0;
                        var angle = Vector3.Angle(forward, dir);
                        if (angle > 2.1f) {
                            this._tran.LookAtLerp(dir.normalized, TimeInfo.DeltaTime * 25);
                            // 距离够了, 但角度不够
                            return NodeStatus.Running;
                        }

                        // 距离够, 角度也够
                        return NodeStatus.Success;
                    }

                    // 如果施法范围内没有获取到目标, 则根据检测范围来获取目标
                    // 获取检测范围 = 技能的检测范围 + 技能本身的施法范围
                    var detectRange = ability.Config.DetectRange + castRange;
                    this._selector
                        .SearcherSphereBody(detectRange)
                        .FilterAlive()
                        .FilterTags(constrainsTags, null)
                        .FilterObstacles()
                        .SelectNearests(targetCount)
                        .Wrap(ability.targets);

                    if (ability.targets.Count != 0)
                        target = ability.targets[0];

                    if (target != null) {
                        var dis = Vector3.Distance(target.transform.Position, this._tran.Position);
                        dis -= castRange;
                        var dir = (target.transform.Position - this._tran.Position).normalized;
                        var point = this._tran.Position + dir * dis;
                        this._control.SetValue(ControlCode.Leap, point);
                        this._control.SetValue(ControlCode.MoveOfPoint, point);
                        // 检测距离够, 但施法距离不够
                        return NodeStatus.Running;
                    }

                    // 施法与检测距离都不够
                    break;
                }
            }

            this.manager.status = CastEvaluateStatus.PickTargetFailure;
            return NodeStatus.Failure;
        }
    }
}
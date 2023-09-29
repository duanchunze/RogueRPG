using System;
using System.Collections;
using System.Collections.Generic;
using MemoryPack;
using Unity.Mathematics;
using UnityEngine;

namespace Hsenl {
    // 例如剑魔的平a
    [Serializable]
    [MemoryPackable()]
    public partial class TpHarmOfTargeted : TpHarm<timeline.HarmOfTargetedInfo> {
        private Faction _faction;

        private List<SelectionTarget> _targetsCache = new();

        protected override void OnNodeOpen() {
            base.OnNodeOpen();
            switch (this.manager.Substantive) {
                case Ability ability: {
                    this._faction = ability.GetHolder()?.GetComponent<Faction>();
                    break;
                }
            }
        }

        protected override void OnTimePointTrigger() {
            switch (this.manager.Substantive) {
                case Ability ability: {
                    if (ability.targets.Count != 0) {
                        var targetCount = GameAlgorithm.MergeCalculateNumeric(this.numerators, NumericType.Ttc);
                        Coroutine.Start(this.HarmTargets1(ability.targets[0], ability.factionTypes, targetCount));
                    }

                    break;
                }
            }
        }

        // private IEnumerator HarmTargets(SelectionTarget target, IList<FactionType> factionTypes, int count) {
        //     var constrainsTags = this._faction.GetTagsOfFactionTypes(factionTypes);
        //     this._targetsCache.Clear();
        //     target.GetComponent<Selections>()?.SelectShpereAliveNearestTargets(8f, count + 1, this._targetsCache, constrainsTags: constrainsTags);
        //
        //     var hurtable = target.Substantive.GetComponent<Hurtable>();
        //     if (hurtable != null) {
        //         this.Harm(hurtable);
        //         FxManager.instance.Play(this.info.HitFx, hurtable.transform.position);
        //         hurtable.GetComponent<Sound>()?.Play(this.info.HitSound);
        //     }
        //
        //     yield return new WaitSeconds(150);
        //
        //     var harmCounter = this._targetsCache.Count;
        //     var startIndex = 0;
        //     var damageRate = 1f;
        //     while (harmCounter > 0) {
        //         for (var i = startIndex; i < startIndex + 3 && i < this._targetsCache.Count; i++) {
        //             var selectTarget = this._targetsCache[i];
        //             harmCounter++;
        //             hurtable = selectTarget.Substantive.GetComponent<Hurtable>();
        //             if (hurtable != null) {
        //                 this.Harm(hurtable, damageRate);
        //                 FxManager.instance.Play(this.info.HitFx, hurtable.transform.position);
        //                 hurtable.GetComponent<Sound>()?.Play(this.info.HitSound);
        //             }
        //         }
        //
        //         startIndex += 3;
        //         damageRate -= 0.2f;
        //         if (damageRate < 0.3f) 
        //             damageRate = 0.3f;
        //         yield return new WaitSeconds(150);
        //     }
        // }

        private IEnumerator HarmTargets1(SelectionTarget target, IList<FactionType> factionTypes, int count) {
            var constrainsTags = this._faction.GetTagsOfFactionTypes(factionTypes);
            using var currentTargets = ListComponent<SelectionTarget>.Create();
            using var buffers = HashSetComponent<SelectionTarget>.Create();
            using var harmedTargets = HashSetComponent<SelectionTarget>.Create();
            currentTargets.Add(target);
            count--;

            var damageRate = 1f;
            while (currentTargets.Count > 0) {
                var volume = 1f;
                Sound sound = null;
                foreach (var currentTarget in currentTargets) {
                    harmedTargets.Add(currentTarget);
                    var hurtable = currentTarget.Substantive.GetComponent<Hurtable>();
                    if (hurtable != null) {
                        this.Harm(hurtable, damageRate);
                        FxManager.Instance.Play(this.info.HitFx, hurtable.transform.Position);

                        float v = 0;
                        var dis = math.distance(hurtable.transform.Position, target.transform.Position);
                        if (dis != 0) {
                            v = 1.8f / dis;
                            v = math.clamp(v, 0.3f, 1f);
                        }

                        volume = (volume + v) * 0.5f;

                        sound ??= hurtable.GetComponent<Sound>();
                    }
                }

                // 没轮只播放一次音效, 不然会产生音爆, 吵耳朵
                // todo 使用3d音效来实现?
                sound?.Play(this.info.HitSound, volume);

                buffers.Clear();
                foreach (var currentTarget in currentTargets) {
                    if (count <= 0)
                        break;

                    this._targetsCache.Clear();
                    currentTarget.GetComponent<Selector>()?.SelectShpereAliveNearestTargets(2.1f, -1, this._targetsCache, constrainsTags: constrainsTags);
                    var maxBuffer = (this._targetsCache.Count / 2);
                    if (maxBuffer > 3)
                        maxBuffer = 3;
                    foreach (var selectionTarget in this._targetsCache) {
                        if (count == 0)
                            break;

                        if (!buffers.Contains(selectionTarget) && !harmedTargets.Contains(selectionTarget)) {
                            count--;
                            buffers.Add(selectionTarget);
                            if (buffers.Count > maxBuffer) {
                                break;
                            }
                        }
                    }
                }

                currentTargets.Clear();
                foreach (var buffer in buffers) {
                    currentTargets.Add(buffer);
                }

                damageRate -= 0.2f;
                if (damageRate < 0.3f)
                    damageRate = 0.3f;

                yield return new WaitSeconds(150);
            }
        }

        private IEnumerator HarmTargets2(SelectionTarget target, IList<FactionType> factionTypes, int count) {
            var constrainsTags = this._faction.GetTagsOfFactionTypes(factionTypes);
            using var harmedTargets = HashSetComponent<SelectionTarget>.Create();
            var currentTarget = target;
            count--;

            var damageRate = 1f;
            while (currentTarget != null) {
                harmedTargets.Add(currentTarget);
                var hurtable = currentTarget.Substantive.GetComponent<Hurtable>();
                if (hurtable != null) {
                    this.Harm(hurtable, damageRate);
                    FxManager.Instance.Play(this.info.HitFx, hurtable.transform.Position);
                    hurtable.GetComponent<Sound>()?.Play(this.info.HitSound);
                }

                if (count <= 0)
                    break;

                this._targetsCache.Clear();
                currentTarget.GetComponent<Selector>()?.SelectShpereAliveNearestTargets(2.5f, -1, this._targetsCache, constrainsTags: constrainsTags);
                currentTarget = null;
                foreach (var selectionTarget in this._targetsCache) {
                    if (!harmedTargets.Contains(selectionTarget)) {
                        count--;
                        currentTarget = selectionTarget;
                        break;
                    }
                }

                // damageRate -= 0.2f;
                // if (damageRate < 0.3f)
                //     damageRate = 0.3f;

                yield return new WaitSeconds(120);
            }
        }
    }
}
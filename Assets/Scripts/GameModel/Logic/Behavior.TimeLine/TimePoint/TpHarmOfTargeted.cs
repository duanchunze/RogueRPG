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

        protected override void OnNodeOpen() {
            base.OnNodeOpen();
            switch (this.manager.Bodied) {
                case Ability ability: {
                    this._faction = this.manager.AttachedBodied?.GetComponent<Faction>();
                    break;
                }
            }
        }

        protected override void OnTimePointTrigger() {
            switch (this.manager.Bodied) {
                case Ability ability: {
                    if (ability.targets.Count != 0) {
                        var targetCount = GameAlgorithm.MergeCalculateNumeric(this.numerators, NumericType.Ttc);
                        Coroutine.Start(this.HarmTargets(ability.targets[0], ability.factionTypes, targetCount));
                    }

                    break;
                }
            }
        }

        private IEnumerator HarmTargets(SelectionTarget target, IList<FactionType> factionTypes, int count) {
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
                    var hurtable = currentTarget.Bodied.GetComponent<Hurtable>();
                    if (hurtable != null) {
                        this.Harm(hurtable, this.info.HarmFormula, damageRate);
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

                    var targets = currentTarget.GetComponent<Selector>()?
                        .SearcherSphereBody(2.1f)
                        .FilterAlive()
                        .FilterTags(constrainsTags, null)
                        .SelectNearests(-1).Targets;

                    if (targets == null)
                        continue;

                    var maxBuffer = (targets.Count / 2);
                    if (maxBuffer > 3)
                        maxBuffer = 3;
                    for (int i = 0, len = targets.Count; i < len; i++) {
                        if (count == 0)
                            break;

                        var selectionTarget = targets[i];
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
    }
}
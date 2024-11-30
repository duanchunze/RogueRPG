using System;
using System.Collections;
using System.Collections.Generic;
using MemoryPack;

namespace Hsenl {
    // 例如剑魔的平a
    [Serializable]
    [MemoryPackable]
    public partial class TpHarmOfTargeted : TpHarm<timeline.HarmOfTargetedInfo> {
        private Faction _faction;

        protected override void OnEnable() {
            base.OnEnable();
            switch (this.manager.Bodied) {
                case Ability ability: {
                    this._faction = this.manager.Bodied.MainBodied?.GetComponent<Faction>();
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

        private IEnumerator HarmTargets(SelectionTargetDefault target, IList<FactionType> factionTypes, int count) {
            const int maxBuffer = 3; // 每次波及时的最大缓冲数
            const int internalTime = 150; // 每次波及的间隔时间(ms)
            
            var constrainsTags = this._faction.GetTagsOfFactionTypes(factionTypes);
            using var currentTargets = ListComponent<SelectionTarget>.Rent();
            using var buffers = HashSetComponent<SelectionTarget>.Create();
            using var harmedTargets = HashSetComponent<SelectionTarget>.Create();
            currentTargets.Add(target);
            count--;

            var damageRate = 1f;
            while (currentTargets.Count > 0) {
                var volume = 1f;
                Hurtable soundHurtable = null;
                foreach (var currentTarget in currentTargets) {
                    harmedTargets.Add(currentTarget);
                    var hurtable = currentTarget.Bodied.GetComponent<Hurtable>();
                    if (hurtable != null) {
                        this.Harm(hurtable, this.info.HarmFormula, damageRate);

                        float v = 0;
                        var dis = Vector3.Distance(hurtable.transform.Position, target.transform.Position);
                        if (dis != 0) {
                            v = 1.8f / dis;
                            v = Math.Clamp(v, 0.3f, 1f);
                        }

                        volume = (volume + v) * 0.5f;

                        soundHurtable ??= hurtable;
                    }
                }

                buffers.Clear();
                foreach (var currentTarget in currentTargets) {
                    if (count <= 0)
                        break;

                    var targets = currentTarget.GetComponent<SelectorDefault>()?
                        .SearcherSphereBody(2.1f)
                        .FilterAlive()
                        .FilterTags(constrainsTags, null)
                        .SelectNearests(int.MaxValue).Targets;

                    if (targets == null)
                        continue;

                    var max = (targets.Count / 2);
                    if (max > maxBuffer)
                        max = maxBuffer;
                    for (int i = 0, len = targets.Count; i < len; i++) {
                        if (count == 0)
                            break;

                        var selectionTarget = targets[i];
                        if (!buffers.Contains(selectionTarget) && !harmedTargets.Contains(selectionTarget)) {
                            count--;
                            buffers.Add(selectionTarget);
                            if (buffers.Count > max) {
                                break;
                            }
                        }
                    }
                }

                currentTargets.Clear();
                foreach (var buffer in buffers) {
                    currentTargets.Add(buffer);
                }

                // 每次波及, 伤害递减0.2, 最低降到0.3
                damageRate -= 0.2f;
                if (damageRate < 0.3f)
                    damageRate = 0.3f;

                yield return new WaitSeconds(internalTime);
            }
        }
    }
}
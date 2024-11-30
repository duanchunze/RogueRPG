using System;
using System.Collections.Generic;
using System.Linq;
using Hsenl.ability;
using MemoryPack;

namespace Hsenl {
    [Serializable]
    [MemoryPackable(GenerateType.CircularReference)]
    public partial class DrawCard : Bodied {
        private Action _onSelectFinish;

        public event Action OnSelectFinish {
            add => this._onSelectFinish += value;
            remove => this._onSelectFinish -= value;
        }


        public async HTask StartDrawCard() {
            // var patchs = this.RandomPatchs1();
            var patchs = this.RandomPatchs2();
            if (patchs.Length == 0)
                return;

            this.FillInNews(patchs);
            await InvokeStation.InvokeDrawCard(this);
        }

        public Bodied[] GetCandidates() {
            using var list = ListComponent<Bodied>.Rent();
            foreach (var bodied in this.FindBodiedsInIndividual<Bodied>()) {
                switch (bodied) {
                    case Ability:
                    case AbilityPatch: {
                        list.Add(bodied);
                        break;
                    }
                }
            }

            return list.ToArray();
        }

        // 随机方案1
        public AbilityPatch[] RandomPatchs1() {
            var abibar = this.MainBodied.FindBodiedInIndividual<AbilitesBar>();
            using var weights = ListComponent<int>.Rent();
            using var candidates = ListComponent<AbilityPatchConfig>.Rent();
            // 遍历所有的补丁, 对每个补丁进行判断, 只要符合要求的, 都添加到候选人里, 供后续随机
            foreach (var abilityPatchConfig in Tables.Instance.TbAbilityPatchConfig.DataList) {
                var abi = abibar.FindAbility(abilityPatchConfig.TargetAbility);
                if (abi == null)
                    continue;

                candidates.Add(abilityPatchConfig);
                weights.Add(1);
            }

            var randomPatchConfigs = RandomHelper.RandomArrayOfWeight(candidates, weights, weights.Count);
            using var patchs = ListComponent<AbilityPatch>.Rent();
            foreach (var patchConfig in randomPatchConfigs) {
                var patch = AbilityPatchFactory.Create(patchConfig);
                patchs.Add(patch);
            }

            return patchs.ToArray();
        }

        public Bodied[] RandomPatchs2() {
            var abibar = this.MainBodied.FindBodiedInIndividual<AbilitesBar>();
            var totalCandidates = ListComponent<Bodied>.Rent();
            var abiCount = 0;
            var abiPatchs = abibar.FindBodiedsInIndividual<AbilityPatch>();

            // 每个技能都有自己的补丁随机池, 且只能选一个
            var allPatchConfigs = Tables.Instance.TbCardSingletonConfig.AbilityPatchPool.Select(x => Tables.Instance.TbAbilityPatchConfig.GetByAlias(x))
                .ToArray();
            foreach (var abi in abibar.ExplicitAbilies) {
                using var weights = ListComponent<int>.Rent();
                using var candidates = ListComponent<AbilityPatchConfig>.Rent();
                foreach (var patchConfig in allPatchConfigs) {
                    // 该技能的补丁
                    if (abi.Name != patchConfig.TargetAbility)
                        continue;

                    // 判断是否符合该补丁要求的最低等级
                    if (patchConfig.LowestLv != -1) {
                        var abiLv = abi.GetComponent<Numerator>().GetValue(NumericType.LV);
                        if (abiLv < patchConfig.LowestLv)
                            continue;
                    }

                    // 满足前置补丁条件, 至少满足一个
                    if (patchConfig.PreconditionPatchs.Count != 0) {
                        var succ = false;
                        foreach (var preconditionPatch in patchConfig.PreconditionPatchs) {
                            var has = ContainsPatch(abiPatchs, preconditionPatch);
                            if (has) {
                                succ = true;
                                break;
                            }
                        }

                        if (!succ)
                            continue;
                    }

                    // 排斥补丁, 如果有一个, 都不行
                    if (patchConfig.RepulsionPatchs.Count != 0) {
                        var succ = true;
                        foreach (var repulsionPatch in patchConfig.RepulsionPatchs) {
                            var has = ContainsPatch(abiPatchs, repulsionPatch);
                            if (has) {
                                succ = false;
                                break;
                            }
                        }

                        if (!succ)
                            continue;
                    }

                    // 有没有超出该补丁允许拥有的最大个数
                    if (patchConfig.MaxCount != -1) {
                        var count = 0;
                        for (int i = 0; i < abiPatchs.Length; i++) {
                            var patch = abiPatchs[i];
                            if (patch.Name == patchConfig.Alias)
                                count++;
                        }

                        if (count >= patchConfig.MaxCount)
                            continue;
                    }

                    candidates.Add(patchConfig);
                    weights.Add(1);
                }

                abiCount++;
                if (candidates.Count != 0) {
                    var randomPatchConfigs = RandomHelper.RandomArrayOfWeight(candidates, weights, 1);
                    var patch = AbilityPatchFactory.Create(randomPatchConfigs[0]);
                    patch.Entity.Active = false;
                    patch.RealTargetAbility = abi;
                    totalCandidates.Add(patch);
                }
            }

            // 如果不足, 则继续补充技能
            var replenishCount = abibar.ExplicitAbilityCapacity - abiCount;
            if (replenishCount > 0) {
                using var list = ListComponent<string>.Rent();
                foreach (var alias in Tables.Instance.TbCardSingletonConfig.AbilityPool) {
                    // 如果已经有该技能了, 则不会再抽取了
                    var abi = abibar.FindAbility(alias);
                    if (abi != null)
                        continue;

                    list.Add(alias);
                }

                if (list.Count > replenishCount) {
                    var randomConfigs = RandomHelper.RandomArrayOfWeight(list, replenishCount, 1);
                    foreach (var config in randomConfigs) {
                        var abi = AbilityFactory.Create(config);
                        abi.Entity.Active = false;
                        totalCandidates.Add(abi);
                    }
                }
                else {
                    foreach (var abilityConfig in list) {
                        var abi = AbilityFactory.Create(abilityConfig);
                        abi.Entity.Active = false;
                        totalCandidates.Add(abi);
                    }
                }
            }

            if (totalCandidates.Count <= 4) {
                return totalCandidates.ToArray();
            }

            var results = RandomHelper.RandomArrayOfWeight(totalCandidates, 4, 1);

            return results;

            bool ContainsPatch(AbilityPatch[] abilityPatches, string alias) {
                for (int i = 0; i < abilityPatches.Length; i++) {
                    var patch = abilityPatches[i];
                    if (patch.Name == alias)
                        return true;
                }

                return false;
            }
        }

        public void FillInNews(IList<Bodied> values) {
            DestroyChildren(this.Entity);

            for (int i = 0; i < values.Count; i++) {
                var patch = values[i];
                patch.SetParent(this.Entity);
            }
        }

        public void SelectCandidate(Bodied bodied) {
            switch (bodied) {
                case AbilityPatch abilityPatch: {
                    var abi = abilityPatch.RealTargetAbility;
                    if (abi == null) {
                        var abibar = this.MainBodied.FindBodiedInIndividual<AbilitesBar>();
                        abi = abibar.FindAbility(abilityPatch.Config.TargetAbility);
                        if (abi == null)
                            return;
                    }

                    abi.AddPatch(abilityPatch);
                    break;
                }

                case Ability ability: {
                    var abibar = this.MainBodied.FindBodiedInIndividual<AbilitesBar>();
                    abibar.EquipAbility(ability);
                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }

            try {
                this._onSelectFinish?.Invoke();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }
    }
}
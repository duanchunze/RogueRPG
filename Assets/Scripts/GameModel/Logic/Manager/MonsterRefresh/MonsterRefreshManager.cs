using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Hsenl {
    public class MonsterRefreshManager : MonoBehaviour {
        public List<UnityEngine.Transform> refreshGroups;

        public UnityEngine.Transform aliveHolder;

        private int _currentGroupIndex;

        public float hpRatio = 1;

        public bool IsOver => this._currentGroupIndex == this.refreshGroups.Count && this.aliveHolder.childCount == 0;

        [Button("刷新怪物")]
        public bool Refresh() {
            if (this._currentGroupIndex >= this.refreshGroups.Count)
                return false;

            var group = this.refreshGroups[this._currentGroupIndex++];
            using var list = ListComponent<ActorSpawnPoint>.Create();
            for (int i = 0, len = group.childCount; i < len; i++) {
                var spawnPoint = group.GetChild(i);
                if (spawnPoint.gameObject.activeSelf == false)
                    continue;

                var actor = spawnPoint.GetComponent<ActorSpawnPoint>();
                list.Add(actor);
            }

            foreach (var spawnPoint in list) {
                var actor = spawnPoint.Spawn();
                var numerator = actor.GetComponent<Numerator>();
                var config = actor.Config;
                var numericConfig = Tables.Instance.TbNumericActorConfig.GetByAlias(config.NumericAlias);
                if (numericConfig.TryGetNum(NumericType.MaxHp, out var maxhp)) {
                    // 按比例重设血量
                    var hpType = maxhp.Type;
                    maxhp *= this.hpRatio;
                    maxhp.Convert(hpType);
                    numerator.SetRawValue(NumericType.MaxHp, maxhp);
                    numerator.SetRawValue(NumericType.Hp, maxhp);
                }

                actor.SetMonoParent(this.aliveHolder);
            }

            return true;
        }
    }
}
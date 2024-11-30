using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Hsenl {
    public class MonsterRefresher : MonoBehaviour {
        public GameObject refreshHolder;
        private readonly List<UnityEngine.Transform> _refreshGroups = new();

        public UnityEngine.Transform aliveHolder;

        private int _currentGroupIndex;

        public float hpRatio = 1;

        public int AliveCount => this.aliveHolder.childCount;

        public bool IsOver => this._currentGroupIndex == this._refreshGroups.Count && this.aliveHolder.childCount == 0;

        private void Awake() {
            this._refreshGroups.Clear();
            var holder = this.refreshHolder.transform;
            var childCount = holder.childCount;
            for (int i = 0; i < childCount; i++) {
                var child = holder.GetChild(i);
                if (!child.gameObject.activeSelf)
                    continue;

                this._refreshGroups.Add(child);
            }
        }

        [Button("刷新怪物")]
        public bool Refresh(out List<Actor> actors) {
            if (this._currentGroupIndex >= this._refreshGroups.Count) {
                actors = new();
                return false;
            }

            var group = this._refreshGroups[this._currentGroupIndex++];
            using var list = ListComponent<ActorSpawnPoint>.Rent();
            for (int i = 0, len = group.childCount; i < len; i++) {
                var spawnPoint = group.GetChild(i);
                if (spawnPoint.gameObject.activeSelf == false)
                    continue;

                var actor = spawnPoint.GetComponent<ActorSpawnPoint>();
                list.Add(actor);
            }

            actors = new();
            foreach (var spawnPoint in list) {
                var actor = spawnPoint.Spawn();
                var numerator = actor.GetComponent<Numerator>();
                var config = actor.Config;
                var numericConfig = config.NumericActorConfig;
                if (numericConfig.TryGetNum(NumericType.MaxHp, out var maxhp)) {
                    // 按比例重设血量
                    var hpType = maxhp.Type;
                    maxhp *= this.hpRatio;
                    maxhp.Convert(hpType);
                    numerator.SetRawValue(NumericType.MaxHp, maxhp);
                    numerator.SetRawValue(NumericType.Hp, maxhp);
                }

                actor.SetMonoParent(this.aliveHolder);
                actors.Add(actor);
            }

            return true;
        }
    }
}
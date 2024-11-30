using System;
using UnityEngine.Serialization;

namespace Hsenl {
    public class LevelManagerKillAll : LevelManager {
        public MonsterRefresher monsterRefresher;
        public bool IsPassed => this.monsterRefresher.IsOver;

        public override void Begin() {
            this.monsterRefresher.hpRatio = this.monsterHpRatio;
            this.monsterRefresher.Refresh(out var _);
        }

        private void Update() {
            if (this.IsPassed) {
                this.enabled = false;
                this.Pass();
            }
        }
    }
}
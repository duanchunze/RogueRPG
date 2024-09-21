using System;

namespace Hsenl {
    public class CheckpointManagerKillAll : CheckpointManager {
        public MonsterRefreshManager monsterRefreshManager;
        public bool IsPassed => this.monsterRefreshManager.IsOver;

        public override void Begin() {
            this.monsterRefreshManager.hpRatio = this.monsterHpRatio;
            this.monsterRefreshManager.Refresh();
        }

        private void Update() {
            if (this.IsPassed) {
                this.enabled = false;
                this.onCheckpointPassed?.Invoke();
            }
        }
    }
}
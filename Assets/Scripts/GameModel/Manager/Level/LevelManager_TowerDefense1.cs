using UnityEngine;

namespace Hsenl {
    public class LevelManager_TowerDefense1 : LevelManager {
        public int towerId;
        public GameObject towerSpawnPoint;
        public MonsterRefresher monsterRefresher;
        private Actor _tower;
        public bool IsPassed => this.monsterRefresher.IsOver;

        public override void Begin() {
            this._tower = ActorManager.Instance.Rent(this.towerId, this.towerSpawnPoint.transform.position);
            this.monsterRefresher.hpRatio = this.monsterHpRatio;
            this.monsterRefresher.Refresh(out var actors);
            foreach (var actor in actors) {
                var selector = actor.GetComponent<SelectorDefault>();
                if (selector != null) {
                    selector.PrimaryTarget = this._tower.GetComponent<SelectionTargetDefault>();
                }
            }
        }

        private void Update() {
            if (this._tower == null)
                return;

            if (this.IsPassed) {
                this.enabled = false;
                this.Pass();
            }
            else {
                if (this.monsterRefresher.AliveCount <= 0) {
                    this.Begin();
                }
            }
        }
    }
}
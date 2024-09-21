using System.Collections.Generic;
using MemoryPack;

namespace Hsenl {
    [MemoryPackable]
    public partial class TpSummoningMinion : TpInfo<timeline.SummoningMinionInfo> {
        protected override void OnTimePointTrigger() {
            var self = this.manager.Bodied.MainBodied;
            var minionsBar = self.GetComponent<MinionsBar>();

            var spawnPoint = RandomHelper.NextFloat3(self.transform.Position - Vector3.One, self.transform.Position + Vector3.One);
            var validPoint = NavMeshAgent.CheckIfPointWalkable(spawnPoint, out var hit);
            if (!validPoint) {
                spawnPoint = self.transform.Position;
            }


            Minion minion = null;
            if (minionsBar != null) {
                minion = minionsBar.Rent(this.info.MinionAlias, spawnPoint);
            }
            else {
                minion = ActorManager.Instance.Rent(this.info.MinionAlias, spawnPoint)?.GetOrAddComponent<Minion>();
            }

            if (minion != null) {
                minion.source = this.manager.Bodied;
                var list = this.manager.Blackboard.GetData<List<Minion>>("MaxSummoningNum");
                if (list != null) {
                    list.Add(minion);
                }

                minion.onOver += this.OnMinionOver;
            }
        }

        private void OnMinionOver(Minion m) {
            m.onOver -= this.OnMinionOver;
            if (this.manager == null)
                return;

            var l = this.manager.Blackboard.GetData<List<Minion>>("MaxSummoningNum");
            if (l != null) {
                l.Remove(m);
            }
        }
    }
}
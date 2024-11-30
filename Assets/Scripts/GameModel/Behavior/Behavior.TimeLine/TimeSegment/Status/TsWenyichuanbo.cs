using Hsenl.timeline;
using MemoryPack;

namespace Hsenl {
    [MemoryPackable]
    public partial class TsWenyichuanbo : TsInfo<WenyichuanboInfo> {
        private float _timer;

        protected override void OnTimeSegmentOrigin() { }

        protected override void OnTimeSegmentRunning() {
            this._timer += TimeInfo.DeltaTime;
            if (this._timer > this.info.InternalTime) {
                this._timer = 0;

                switch (this.manager.Bodied) {
                    case Status status: {
                        var faction = status.inflictor.GetComponent<Faction>();
                        var constrainsTags = faction.GetTagsOfFactionType(FactionType.Enemy);
                        var selector = this.manager.Bodied.MainBodied.GetComponent<SelectorDefault>();
                        var targets = selector.SearcherSphereBody(this.info.Chuanbofanwei)
                            .FilterAlive()
                            .FilterTags(constrainsTags, null)
                            .SelectNearests(int.MaxValue).Targets;
                        foreach (var target in targets) {
                            if (target.Bodied == this.manager.Bodied.MainBodied)
                                continue;

                            Shortcut.InflictionStatus(status.inflictor, target.Bodied, status.Name);
                        }

                        break;
                    }
                }
            }
        }

        protected override void OnTimeSegmentTerminate(bool timeout) { }
    }
}
using Hsenl.ai;
using MemoryPack;
using UnityEngine;

namespace Hsenl {
    [MemoryPackable]
    public partial class AILoiterFollowMaster : AIInfo<LoiterFollowMaster> {
        private Minion _minion;
        private Control _control;
        private Vector3 _targetPoint;

        protected override bool Check() {
            return true;
        }

        protected override void Enter() {
            this._minion = this.manager.Bodied.MainBodied.GetComponent<Minion>();
            this._control = this.manager.Bodied.MainBodied.GetComponent<Control>();
        }

        protected override void Running() {
            var owner = this.manager.Bodied.MainBodied;
            var master = this._minion.master;
            var range = Vector3.One * this.info.LoiterRange;

            // 超出距离就跟随主人
            if (Vector3.Distance(owner.transform.Position, master.transform.Position) > this.info.LoiterRange) {
                if (Timer.ClockTick(0.15f)) {
                    this._targetPoint = RandomHelper.NextFloat3(master.transform.Position - range, master.transform.Position + range);
                    Shortcut.SimulatePointMove(this._control, this._targetPoint);
                }
            }
            // 不超出距离, 就游荡
            else {
                if (owner.transform.IsMoveStop()) {
                    if (Timer.ClockTick(7f)) {
                        this._targetPoint = RandomHelper.NextFloat3(master.transform.Position - range, master.transform.Position + range);
                        Shortcut.SimulatePointMove(this._control, this._targetPoint);
                    }
                }
            }
        }

        protected override void Exit() { }
    }
}
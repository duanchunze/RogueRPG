using System;
using MemoryPack;
using Unity.Mathematics;
using UnityEngine;

namespace Hsenl {
    [Serializable]
    [MemoryPackable()]
    public partial class TsForceMovement : TsInfo<timeline.ForceMovementInfo> {
        private float3 _direction;
        private float _speed;

        protected override void OnTimeSegmentOrigin() {
            Transform inflictorTra = null;
            Transform tra = null;
            switch (this.manager.Bodied) {
                case Status status: {
                    inflictorTra = status.inflictor.transform;
                    tra = this.manager.AttachedBodied.transform;
                    break;
                }
            }

            if (tra == null || inflictorTra == null) return;

            this._direction = tra.Position - inflictorTra.Position;
            this._direction = math.normalize(this._direction);
            this._speed = this.info.Distance / this.manager.TillTime;
            tra.MoveToPoint(tra.Position + (Vector3)this._direction * this.info.Distance, this._speed);
        }

        protected override void OnTimeSegmentRunning() { }

        protected override void OnTimeSegmentTerminate(bool timeout) { }
    }
}
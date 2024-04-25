using System;
using MemoryPack;
using Unity.Mathematics;
using UnityEngine;
using DG.Tweening;

namespace Hsenl {
    [Serializable]
    [MemoryPackable()]
    public partial class TsForceMovement : TsInfo<timeline.ForceMovementInfo> {
        private float3 _direction;
        private float _speed;
        private Transform _transform;

        protected override void OnTimeSegmentOrigin() {
            Transform inflictorTra = null;
            this._transform = null;
            switch (this.manager.Bodied) {
                case Status status: {
                    inflictorTra = status.inflictor.transform;
                    this._transform = this.manager.Bodied.AttachedBodied.transform;
                    break;
                }
            }

            if (this._transform == null || inflictorTra == null) return;

            this._direction = this._transform.Position - inflictorTra.Position;
            this._direction = math.normalize(this._direction);
            this._speed = this.info.Distance / this.manager.TillTime;
            var targetPoint = this._transform.Position + (Vector3)this._direction * this.info.Distance;
            // this._transform.MoveToPoint(targetPoint, this._speed);

            DOTween.To(() => this._transform.Position, (x) => { this._transform.Position = x; }, targetPoint, this._speed).SetSpeedBased(true);
        }

        protected override void OnTimeSegmentRunning() { }

        protected override void OnTimeSegmentTerminate(bool timeout) { }
    }
}
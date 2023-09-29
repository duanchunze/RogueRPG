using System;
using Unity.Mathematics;
using UnityEngine;

namespace Hsenl {
    [Serializable]
    public class Bolt : Substantive, IUpdate {
        public string boltName;
        public BoltType boltType;
        public float ballisticSpeed;
        public Action onDormancy;

        private Action _onReached;
        private Action<Substantive> _onCollision;
        public Substantive shooter;
        private Transform _target;
        private float3 _targetPoint;
        private Vector3 _direction;
        private float _maxDistance;
        private float3 _origin;

        private Transform _selfTra;

        protected override void OnAwake() {
            this._selfTra = this.transform;
        }

        public void Reset() {
            this.boltType = BoltType.None;
            this.ballisticSpeed = 0;
            this.onDormancy = null;
            this._onReached = null;
            this._onCollision = null;
            this.shooter = null;
            this._target = null;
            this._targetPoint = default;
            this._direction = default;
            this._maxDistance = 0;
            this._origin = default;
        }

        public void FireOfPoint(float3 orig, float3 dest, float speed, Action onReached) {
            this.boltType = BoltType.PointTo;
        }

        public void FireOfTarget(float3 orig, Transform target, float speed, Action onReached) {
            this.boltType = BoltType.TargetTo;
            this._origin = orig;
            this.transform.Position = orig;
            this._target = target;
            this.ballisticSpeed = speed;
            this._onReached = onReached;
            this.Entity.Active = true;
        }

        public void FireOfDirection(float3 orig, float3 dir, float maxDistance, float speed, Action<Substantive> onCollision) {
            this.boltType = BoltType.DirectionTo;
            this._origin = orig;
            this.transform.Position = orig;
            this._direction = dir;
            this._maxDistance = maxDistance * maxDistance;
            this.ballisticSpeed = speed;
            this._onCollision = onCollision;
            this.Entity.Active = true;

            var collider = this.GetComponent<SphereCollider>();
            if (collider == null) {
                collider = ColliderFactory.CreateCollider<SphereCollider>(this.Entity, GameColliderPurpose.BodyTrigger);
                collider.SetTriggerEnterListening(col => { onCollision?.Invoke(col.Substantive); });
            }
        }

        public void Update() {
            switch (this.boltType) {
                case BoltType.None: break;
                case BoltType.PointTo: {
                    var pos = (float3)this._selfTra.Position;
                    var dir = math.normalize(this._targetPoint - pos);
                    this._selfTra.Translate(dir * (this.ballisticSpeed * TimeInfo.DeltaTime));
                    if (math.distancesq(this._origin, pos) > math.distancesq(this._origin, this._targetPoint)) {
                        this._onReached?.Invoke();
                        this.Dormancy();
                    }

                    break;
                }
                case BoltType.TargetTo: {
                    var pos = (float3)this._selfTra.Position;
                    var targetPos = (float3)this._target.Position;
                    var dir = math.normalize(targetPos - pos);
                    this._selfTra.Translate(dir * (this.ballisticSpeed * TimeInfo.DeltaTime));
                    if (math.distancesq(this._selfTra.Position, this._target.Position) < 0.25f) {
                        this._onReached?.Invoke();
                        this.Dormancy();
                    }

                    break;
                }
                case BoltType.DirectionTo: {
                    var pos = (float3)this._selfTra.Position;
                    this._selfTra.Translate(this._direction * (this.ballisticSpeed * TimeInfo.DeltaTime));
                    if (math.distancesq(this._origin, pos) > this._maxDistance) {
                        this._onReached?.Invoke();
                        this.Dormancy();
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Dormancy() {
            this.onDormancy?.Invoke();
            BoltManager.Instance.Return(this);
            this.Reset();
        }
    }
}
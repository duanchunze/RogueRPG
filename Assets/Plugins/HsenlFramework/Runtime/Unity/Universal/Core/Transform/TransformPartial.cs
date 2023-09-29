using MemoryPack;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

namespace Hsenl {
    public partial class Transform {
        [MemoryPackIgnore]
        private UnityEngine.Transform _transform;

        [MemoryPackIgnore]
        private NavMeshAgent _meshAgent;

        private NavMeshAgent NavMeshAgent {
            get {
                this._meshAgent ??= this._transform.GetComponent<NavMeshAgent>();
                return this._meshAgent;
            }
        }

        [MemoryPackIgnore]
        public Vector3 Position {
            get => this.UnityTransformGetter().position;
            set => this.UnityTransformGetter().position = value;
        }

        [MemoryPackIgnore]
        public Vector3 LocalPosition {
            get => this.UnityTransformGetter().localPosition;
            set => this.UnityTransformGetter().localPosition = value;
        }

        [MemoryPackIgnore]
        public Quaternion Quaternion {
            get => this.UnityTransformGetter().rotation;
            set => this.UnityTransformGetter().rotation = value;
        }

        [MemoryPackIgnore]
        public Quaternion LocalRotation {
            get => this.UnityTransformGetter().localRotation;
            set => this.UnityTransformGetter().localRotation = value;
        }

        [MemoryPackIgnore]
        public Vector3 LocalScale {
            get => this.UnityTransformGetter().localScale;
            set => this.UnityTransformGetter().localScale = value;
        }

        [MemoryPackIgnore]
        public Vector3 Forward {
            get => this.UnityTransformGetter().forward;
            set => this.UnityTransformGetter().forward = value;
        }

        private UnityEngine.Transform UnityTransformGetter() => this._transform ??= this.Entity.UnityTransform;

        public void SetPosition(Vector3 position) {
            if (this._meshAgent) {
                this._meshAgent.isStopped = true;
                this._meshAgent.enabled = false;
            }

            this.Position = position;

            if (this._meshAgent) {
                this._meshAgent.enabled = true;
            }
        }

        public void Translate(Vector3 translation) {
            this._transform.Translate(translation, Space.World);
        }

        public bool IsNavMoveDone() {
            if (this.NavMeshAgent is not { isOnNavMesh: true })
                return true;

            return this.NavMeshAgent.remainingDistance <= this.NavMeshAgent.stoppingDistance;
        }

        public void MoveToPoint(Vector3 point, float speed, float stopDis = 0.05f) {
            if (!this.NavMeshAgent.isOnNavMesh) {
                // 如果出现代理不在寻路网格上的话, 就重开一下.
                this.NavMeshAgent.enabled = false;
            }

            this.NavMeshAgent.enabled = true;
            this.NavMeshAgent.destination = point;
            this.NavMeshAgent.speed = speed;
            this.NavMeshAgent.stoppingDistance = stopDis;
        }

        public void MoveToPoint(Vector3 point) {
            if (!this.NavMeshAgent.isOnNavMesh)
                this.NavMeshAgent.enabled = false;

            this.NavMeshAgent.enabled = true;

            if (this.NavMeshAgent.isOnNavMesh)
                this.NavMeshAgent.destination = point;
        }

        public void LookAtLerp(float3 forward, float t) {
            var forwardRotation = quaternion.LookRotation(forward, math.up());
            this.Quaternion = math.nlerp(this.Quaternion, forwardRotation, t);
        }

        public void LookAt(float3 forward) {
            var forwardRotation = quaternion.LookRotation(forward, math.up());
            this.Quaternion = forwardRotation;
        }

        public void LookAtPointLerp(float3 point, float t) {
            var position = (float3)this.Position;
            if (point.Equals(position)) return;

            var forward = point - position;
            var forwardRotation = quaternion.LookRotation(forward, math.up());
            this.Quaternion = math.nlerp(this.Quaternion, forwardRotation, t);
        }

        public void LookAtPoint(float3 point) {
            var position = (float3)this.Position;
            if (point.Equals(position)) return;

            var forward = point - position;
            var forwardRotation = quaternion.LookRotation(forward, math.up());
            this.Quaternion = forwardRotation;
        }
    }
}
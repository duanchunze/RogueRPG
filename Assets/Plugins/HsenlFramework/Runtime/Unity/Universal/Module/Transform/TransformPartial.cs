using MemoryPack;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

namespace Hsenl {
    public partial class Transform {
        [MemoryPackIgnore]
        private UnityEngine.Transform _transform;

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

        public void Translate(Vector3 translation) {
            this.UnityTransformGetter().Translate(translation, Space.World);
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

        #region NavMesh // 因为当前游戏的移动都是采用的navmesh来驱动的, 所以干脆就在transform里写了快捷方式

        private NavMeshAgent _navMeshAgent;

        public NavMeshAgent NavMeshAgent => this._navMeshAgent ??= this.GetComponent<NavMeshAgent>();

        public bool IsNavMoveDone() {
            var agent = this.NavMeshAgent;
            if (agent == null)
                return true;

            return agent.IsNavMoveDone();
        }

        public void StopMove() {
            var agent = this.NavMeshAgent;
            if (agent == null)
                return;

            if (agent.IsOnNavMesh)
                agent.IsStopped = true;
        }

        public void SetPosition(Vector3 position) {
            var agent = this.NavMeshAgent;
            if (agent == null)
                return;

            agent.SetPosition(position);
        }

        public void MoveToPoint(Vector3 point, float speed, float stopDis = 0.05f) {
            var agent = this.NavMeshAgent;
            if (agent == null)
                return;

            agent.MoveToPoint(point, speed, stopDis);
        }

        public void MoveToPoint(Vector3 point) {
            var agent = this.NavMeshAgent;
            if (agent == null)
                return;

            agent.MoveToPoint(point);
        }

        #endregion
    }
}
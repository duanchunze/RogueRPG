using MemoryPack;
using UnityEngine;

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

        [MemoryPackIgnore]
        public Vector3 Right {
            get => this.UnityTransformGetter().right;
            set => this.UnityTransformGetter().right = value;
        }

        private UnityEngine.Transform UnityTransformGetter() => this._transform ??= this.Entity.UnityTransform;

        public void NormalizeValue() {
            this.LocalPosition = Vector3.Zero;
            this.LocalRotation = Quaternion.Identity;
            this.LocalScale = Vector3.One;
        }

        public void Translate(Vector3 translation) {
            this.UnityTransformGetter().Translate(translation, Space.World);
        }

        public void LookRotationLerp(Vector3 forward, float t) {
            var forwardRotation = Quaternion.CreateLookRotation(forward, Vector3.Up);
            this.Quaternion = Quaternion.Lerp(this.Quaternion, forwardRotation, t);
        }

        public void LookRotation(Vector3 forward) {
            var forwardRotation = Quaternion.CreateLookRotation(forward, Vector3.Up);
            this.Quaternion = forwardRotation;
        }

        #region NavMesh // 因为当前游戏的移动都是采用的navmesh来驱动的, 所以干脆就在transform里写了快捷方式

        private NavMeshAgent _navMeshAgent;

        public NavMeshAgent NavMeshAgent => this._navMeshAgent ??= this.GetComponent<NavMeshAgent>();

        public bool IsMoveStop() {
            var agent = this.NavMeshAgent;
            if (agent == null)
                return true;

            return agent.IsNavMoveStop();
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
using MemoryPack;
using UnityEngine;
using UnityEngine.AI;

namespace Hsenl {
    public partial class NavMeshAgent {
        [MemoryPackIgnore]
        private UnityEngine.AI.NavMeshAgent _navMeshAgent;

        [MemoryPackIgnore]
        public bool IsStopped {
            get => this._navMeshAgent.isStopped;
            set => this._navMeshAgent.isStopped = value;
        }

        [MemoryPackIgnore]
        public bool IsOnNavMesh {
            get => this._navMeshAgent.isOnNavMesh;
        }

        [MemoryPackIgnore]
        public Vector3 Destination {
            get => this._navMeshAgent.destination;
            set => this._navMeshAgent.destination = value;
        }

        [MemoryPackIgnore]
        public float Speed {
            get => this._navMeshAgent.speed;
            set => this._navMeshAgent.speed = value;
        }

        [MemoryPackIgnore]
        public float AngularSpeed {
            get => this._navMeshAgent.angularSpeed;
            set => this._navMeshAgent.angularSpeed = value;
        }

        [MemoryPackIgnore]
        public float Acceleration {
            get => this._navMeshAgent.acceleration;
            set => this._navMeshAgent.acceleration = value;
        }

        [MemoryPackIgnore]
        public float StoppingDistance {
            get => this._navMeshAgent.stoppingDistance;
            set => this._navMeshAgent.stoppingDistance = value;
        }

        [MemoryPackIgnore]
        public float Height {
            get => this._navMeshAgent.height;
            set => this._navMeshAgent.height = value;
        }

        [MemoryPackIgnore]
        public float Radius {
            get => this._navMeshAgent.radius;
            set => this._navMeshAgent.radius = value;
        }

        [MemoryPackIgnore]
        public Vector3 Velocity {
            get => this._navMeshAgent.velocity;
            set => this._navMeshAgent.velocity = value;
        }

        [MemoryPackIgnore]
        public int AgentTypeID {
            get => this._navMeshAgent.agentTypeID;
            set => this._navMeshAgent.agentTypeID = value;
        }

        [MemoryPackIgnore]
        public int AvoidancePriority {
            get => this._navMeshAgent.avoidancePriority;
            set => this._navMeshAgent.avoidancePriority = value;
        }

        [MemoryPackIgnore]
        public ObstacleAvoidanceType ObstacleAvoidanceType {
            get => this._navMeshAgent.obstacleAvoidanceType;
            set => this._navMeshAgent.obstacleAvoidanceType = value;
        }

        [MemoryPackIgnore]
        public bool PathPending => this._navMeshAgent.pathPending;

        [MemoryPackIgnore]
        public float RemainingDistance => this._navMeshAgent.remainingDistance;

        [MemoryPackIgnore]
        public NavMeshPathStatus PathStatus => this._navMeshAgent.pathStatus;

        [MemoryPackIgnore]
        public Vector3 PathEndPosition => this._navMeshAgent.pathEndPosition;

        [MemoryPackIgnore]
        public bool HasPath => this._navMeshAgent.hasPath;

        protected override void OnDeserialized() {
            this._navMeshAgent = this.GetMonoComponent<UnityEngine.AI.NavMeshAgent>();
            if (this._navMeshAgent == null) {
                this._navMeshAgent = this.Entity.GameObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
            }
        }

        protected override void OnAwake() {
            this._navMeshAgent = this.GetMonoComponent<UnityEngine.AI.NavMeshAgent>();
            if (this._navMeshAgent == null) {
                this._navMeshAgent = this.Entity.GameObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
            }
        }

        protected override void OnEnable() {
            this._navMeshAgent.enabled = true;
        }

        protected override void OnDisable() {
            if (this.Enable) return;
            this._navMeshAgent.enabled = false;
        }

        public bool IsNavMoveStop() {
            if (!this.IsOnNavMesh)
                return true;

            if (this.PathPending)
                return false;

            if (this.IsStopped)
                return true;

            return this.RemainingDistance <= this.StoppingDistance;
        }

        public void SetPosition(Vector3 position) {
            if (!this.Enable)
                return;

            this.IsStopped = true;
            this._navMeshAgent.enabled = false;
            this.transform.Position = position;
            this._navMeshAgent.enabled = true;
        }

        public void MoveToPoint(Vector3 point, float speed, float stopDis = 0.05f) {
            if (!this.Enable)
                return;

            if (!this.IsOnNavMesh) {
                // 如果出现代理不在寻路网格上的话, 就重开一下.
                this._navMeshAgent.enabled = false;
            }

            this._navMeshAgent.enabled = true;

            if (this.IsOnNavMesh) {
                this.IsStopped = false;
                this.Destination = point;
                this.Speed = speed;
                this.StoppingDistance = stopDis;
            }
        }

        public void MoveToPoint(Vector3 point) {
            if (!this.Enable)
                return;

            if (!this.IsOnNavMesh)
                this._navMeshAgent.enabled = false;

            this._navMeshAgent.enabled = true;

            if (this.IsOnNavMesh) {
                this.IsStopped = false;
                this.Destination = point;
            }
        }

        public bool SamplePathPosition(out NavMeshHit hit, float maxDistance, int areaMask = NavMesh.AllAreas) {
            return this._navMeshAgent.SamplePathPosition(areaMask, maxDistance, out hit);
        }

        public static bool SamplePosition(Vector3 targetPosition, out NavMeshHit hit, float maxDistance, int areaMask = NavMesh.AllAreas) {
            return NavMesh.SamplePosition(targetPosition, out hit, maxDistance, areaMask);
        }

        public bool CalculatePath(Vector3 targetPosition, NavMeshPath path) {
            return this._navMeshAgent.CalculatePath(targetPosition, path);
        }
    }
}
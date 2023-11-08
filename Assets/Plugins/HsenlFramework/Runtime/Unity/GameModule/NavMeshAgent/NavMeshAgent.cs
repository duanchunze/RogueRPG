using MemoryPack;
using UnityEngine;

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

        protected override void OnDeserialized() {
            this._navMeshAgent = this.GetMonoComponent<UnityEngine.AI.NavMeshAgent>();
            if (this._navMeshAgent == null) {
                this._navMeshAgent = this.Entity.GameObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
            }
        }

        protected override void OnConstruction() {
            this._navMeshAgent = this.GetMonoComponent<UnityEngine.AI.NavMeshAgent>();
            if (this._navMeshAgent == null) {
                this._navMeshAgent = this.Entity.GameObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
            }
        }

        public bool IsNavMoveDone() {
            return this._navMeshAgent.remainingDistance <= this._navMeshAgent.stoppingDistance;
        }

        public void SetPosition(Vector3 position) {
            this._navMeshAgent.isStopped = true;
            this.transform.Position = position;
        }

        public void MoveToPoint(Vector3 point, float speed, float stopDis = 0.05f) {
            if (!this._navMeshAgent.isOnNavMesh) {
                // 如果出现代理不在寻路网格上的话, 就重开一下.
                this._navMeshAgent.enabled = false;
            }

            this._navMeshAgent.enabled = true;

            if (this._navMeshAgent.isOnNavMesh) {
                this._navMeshAgent.isStopped = false;
                this._navMeshAgent.destination = point;
                this._navMeshAgent.speed = speed;
                this._navMeshAgent.stoppingDistance = stopDis;
            }
        }

        public void MoveToPoint(Vector3 point) {
            if (!this._navMeshAgent.isOnNavMesh)
                this._navMeshAgent.enabled = false;

            this._navMeshAgent.enabled = true;

            if (this._navMeshAgent.isOnNavMesh) {
                this._navMeshAgent.isStopped = false;
                this._navMeshAgent.destination = point;
            }
        }
    }
}
using System;

namespace Hsenl {
    public partial class NavMeshSurface {
        private Unity.AI.Navigation.NavMeshSurface _unityNavMeshSurface;

        private Unity.AI.Navigation.NavMeshSurface UnityNavMeshSurface {
            get {
                this._unityNavMeshSurface ??= UnityEngine.Object.FindObjectOfType<Unity.AI.Navigation.NavMeshSurface>();
                return this._unityNavMeshSurface;
            }
        }

        public bool IsValid() {
            throw new NotImplementedException();
        }
    }
}
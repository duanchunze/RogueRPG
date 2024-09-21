using System;
using System.Collections.Generic;
using MemoryPack;

namespace Hsenl {
    [Serializable]
    public sealed partial class Scene : Object {
        internal readonly string sceneName;

        internal SceneType sceneType;

        [System.NonSerialized] // unity那个 depth limit 10警告
        private List<Entity> _rootEntities = new();

        public string SceneName => this.sceneName;

        public IReadOnlyList<Entity> RootEntities => this._rootEntities;

        internal Scene(string sceneName) {
            this.sceneName = sceneName;
        }

        internal bool AddRootEntity(Entity entity) {
            if (this._rootEntities.Contains(entity))
                return false;

            this._rootEntities.Add(entity);
            return true;
        }

        internal bool RemoveRootEntity(Entity entity) {
            return this._rootEntities.Remove(entity);
        }
    }
}
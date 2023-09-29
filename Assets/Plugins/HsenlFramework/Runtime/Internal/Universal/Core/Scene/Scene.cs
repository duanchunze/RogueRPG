using System;
using System.Collections.Generic;
using MemoryPack;

namespace Hsenl {
    [Serializable]
    public sealed partial class Scene : Object {
        internal readonly string sceneName;

        internal SceneType sceneType;

        internal List<Entity> rootEntities = new();

        public string SceneName => this.sceneName;

        public IReadOnlyList<Entity> RootEntities => this.rootEntities;

        internal Scene(string sceneName) {
            this.sceneName = sceneName;
        }
    }
}
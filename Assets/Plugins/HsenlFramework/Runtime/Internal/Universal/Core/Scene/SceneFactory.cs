using System;

namespace Hsenl {
    internal static class SceneFactory {
        public static Scene Create(string name) {
            var scene = new Scene(name);
            scene.instanceId = Guid.NewGuid().GetHashCode();
            scene.uniqueId = scene.instanceId;

            return scene;
        }
    }
}
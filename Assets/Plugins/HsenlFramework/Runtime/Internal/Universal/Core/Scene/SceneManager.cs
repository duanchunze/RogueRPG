using System;
using System.Collections.Generic;

namespace Hsenl {
    public enum LoadSceneMode {
        Single,
        Additional,
    }

    public enum SceneType {
        Normal,
        DontDestroyOnLoad,
    }

    public static partial class SceneManager {
        internal static Scene activeScene;
        internal static Scene dontDestroyScene;
        internal static readonly Dictionary<string, Scene> scenes = new();

        public static event Action<Scene> OnSceneLoaded;

        public static Scene GetOrLoadDontDestroyScene() {
            if (dontDestroyScene != null) return dontDestroyScene;
            var scene = LoadScene("DontDestroyOnLoad", LoadSceneMode.Additional);
            dontDestroyScene = scene;
            scene.sceneType = SceneType.DontDestroyOnLoad;
            return scene;
        }

        public static void UnloadAllScene() {
            if (activeScene != null) Object.Destroy(activeScene);
            if (dontDestroyScene != null) Object.Destroy(dontDestroyScene);
            foreach (var scene in scenes) {
                Object.Destroy(scene.Value);
            }

            scenes.Clear();
        }

        public static Scene LoadScene(string name, LoadSceneMode mode) {
            Scene scene;
            switch (mode) {
                case LoadSceneMode.Single:
                    if (activeScene != null) Object.Destroy(activeScene);
                    scene = SceneFactory.Create(name);
                    activeScene = scene;
                    break;
                case LoadSceneMode.Additional:
                    scene = SceneFactory.Create(name);
                    activeScene ??= scene;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }

            scene.sceneType = SceneType.Normal;
            scenes.Add(name, scene);
            try {
                OnSceneLoaded?.Invoke(scene);
            }
            catch (Exception e) {
                Log.Error(e);
            }

            return scene;
        }

        public static Scene GetScene(string name) {
            scenes.TryGetValue(name, out var scene);
            return scene;
        }

        internal static void RemoveScene(string name) {
            scenes.TryGetValue(name, out var scene);
            if (activeScene == scene) activeScene = null;
            scenes.Remove(name);
        }

        public static void MoveEntityToScene(Entity entity, Scene scene) {
            Assert.NullReference(entity, $"scene is null '{scene.sceneName}'");
            // 先把自己和父级剥离开来, 再操作
            entity.SetParent(null, scene);
        }
    }
}
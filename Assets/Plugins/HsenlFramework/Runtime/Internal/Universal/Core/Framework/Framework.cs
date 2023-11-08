using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Hsenl {
    [Serializable]
    public class Framework : Singleton<Framework> {
        public static bool AppQuit { get; private set; }

        [SerializeField]
        private bool displayMono;

        public bool DisplayMono => Define.IsEditor && this.displayMono;

        public static void OnAppStart() {
            AppQuit = false;
        }

        public static void OnAppQuit() {
            AppQuit = true;
            // SceneManager.UnloadAllScene();
        }

        public void Update() {
            EventSystemManager.Instance.Update();
        }

        public void LateUpdate() {
            EventSystemManager.Instance.LateUpdate();
        }

        // 清理缓存, 框架内部许多地方使用了缓存机制, 用于提升速度, 但可能会造成内存的浪费, 所以时不时的清理一次
        public static void ClearCache() {
            RandomHelper.ClearCache();
        }
    }
}
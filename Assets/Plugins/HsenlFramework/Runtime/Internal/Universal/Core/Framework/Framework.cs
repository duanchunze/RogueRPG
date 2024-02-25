using System;
using System.Collections.Generic;
using System.Threading;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;
#endif

namespace Hsenl {
    [Serializable]
    public class Framework : Singleton<Framework> {
        public static bool AppQuit { get; private set; }
        public ThreadSynchronizationContext ThreadSynchronizationContext { get; private set; }

#if UNITY_EDITOR
        [SerializeField]
#endif
        private bool displayMono;


        public bool DisplayMono {
#if UNITY_EDITOR
            get => this.displayMono;
#else
            get => false;
#endif
        }

        public void Start() {
            AppQuit = false;
            this.ThreadSynchronizationContext = new ThreadSynchronizationContext();
        }

        public void Destroy() {
            AppQuit = true;
            this.ThreadSynchronizationContext = null;
        }

        public void Update() {
            this.ThreadSynchronizationContext.Update();
            EventSystemManager.Instance.Update();
        }

        public void LateUpdate() {
            EventSystemManager.Instance.LateUpdate();
        }

        // 清理缓存, 框架内部许多地方使用了缓存机制, 用于提升速度, 但可能会造成内存的浪费, 所以时不时的清理一次
        public void ClearCache() {
            RandomHelper.ClearCache();
        }
    }
}
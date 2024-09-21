using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Hsenl {
    // 关卡管理器. 决定通关的规则, 刷怪的规则
    // 一个关卡的组成是由地图环境、怪物生成、其他物件、管理器组成
    [Serializable]
    public abstract class CheckpointManager : MonoBehaviour {
        public Action onCheckpointPassed;

        public float monsterHpRatio = 1;

        public abstract void Begin();

        protected void Pass() {
            try {
                this.onCheckpointPassed?.Invoke();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }
    }
}
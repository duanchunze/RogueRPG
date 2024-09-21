using System;
using MemoryPack;
using UnityEngine;

namespace Hsenl {
    [ShadowFunction]
    [BehaviorNode]
    public abstract partial class AIInfo<T> : AINode, IConfigInfoInitializer<behavior.Info> where T : ai.AIInfo {
        [MemoryPackIgnore]
        public T info;

        public int infoInstanceId;
        
        public Type InfoType => typeof(T);

        protected override void OnAwake() {
            if (this.info == null && this.infoInstanceId != 0) {
                var inf = behavior.Info.GetInfo(this.infoInstanceId);
                if (inf != null) {
                    this.InitInfo(inf);
                }
            }
        }
        
        public void InitInfo(object configInfo) {
            if (configInfo is behavior.Info i) {
                this.InitInfo(i);
            }
        }

        public void InitInfo(behavior.Info configInfo) {
            var t = (T)configInfo;
            this.info = t;
            this.infoInstanceId = t.InstanceId;

            this.OnConfigInfoInit(t);
        }

        protected virtual void OnConfigInfoInit(T arg) { }
    }
}
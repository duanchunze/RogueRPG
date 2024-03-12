using MemoryPack;
using UnityEngine;

namespace Hsenl {
    [ShadowFunction]
    [BehaviorNode]
    public abstract partial class AIInfo<T> : AINode, IConfigInfoInitializer<behavior.Info> where T : ai.AIInfo {
        [MemoryPackIgnore]
        public T info;

        public int infoInstanceId;

        [ShadowFunction]
        protected override void OnAwake() {
            this.OnAwakeShadow(); // 下面这段代码, 被我们放到影子函数里去实现了, 其他的组件也可以照样写到热重载程序集里
            // if (this.info == null && this.infoInstanceId != 0) {
            //     var inf = behavior.Info.GetInfo(this.infoInstanceId);
            //     if (inf != null) {
            //         this.InitInfo(inf);
            //     }
            // }
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
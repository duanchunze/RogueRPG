using System.Collections.Generic;

namespace Hsenl.behavior {
    public partial class Info {
        private static readonly Dictionary<int, behavior.Info> _idInfos = new();

        public int InstanceId { get; private set; }

        partial void PostInit() {
            this.InstanceId = this.GetHashCode();
            _idInfos[this.InstanceId] = this;
        }

        public static void AddInfo(int instanceId, behavior.Info info) {
            _idInfos.Add(instanceId, info);
        }
        
        public static behavior.Info GetInfo(int instanceId) {
            _idInfos.TryGetValue(instanceId, out var result);
            return result;
        }
    }
}
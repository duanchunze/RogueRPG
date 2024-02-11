using System.Collections.Generic;

namespace Hsenl.procedureline {
    public partial class WorkerInfo {
        private static readonly Dictionary<int, WorkerInfo> _idInfos = new();

        public int InstanceId { get; private set; }

        partial void PostInit() {
            this.InstanceId = this.GetHashCode();
            _idInfos[this.InstanceId] = this;
        }

        public static WorkerInfo GetInfo(int instanceId) {
            _idInfos.TryGetValue(instanceId, out var result);
            return result;
        }
    }
}
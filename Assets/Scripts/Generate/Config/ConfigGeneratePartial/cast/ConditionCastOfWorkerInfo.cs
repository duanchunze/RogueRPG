using System.Collections.Generic;

namespace Hsenl.cast {
    public partial class ConditionCastOfWorkerInfo {
        private static readonly Dictionary<int, ConditionCastOfWorkerInfo> _idInfos = new();

        public int InstanceId { get; private set; }

        partial void PostInit() {
            this.InstanceId = this.GetHashCode();
            _idInfos[this.InstanceId] = this;
        }

        public static ConditionCastOfWorkerInfo GetInfo(int instanceId) {
            _idInfos.TryGetValue(instanceId, out var result);
            return result;
        }
    }
}
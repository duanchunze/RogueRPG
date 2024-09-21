namespace Hsenl.procedureline {
    public partial class AdditionalStatusOnAbilityDamageInfo2 {
        partial void PostInit() {
            var instanceId = this.GetHashCode();
            behavior.Info.AddInfo(instanceId, this.Action); // 因为自己用到了behavior.Action, 所以把这个action也缓存起来.
        }
    }
}
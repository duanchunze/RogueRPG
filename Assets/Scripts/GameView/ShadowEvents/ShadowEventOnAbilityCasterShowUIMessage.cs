namespace Hsenl.View {
    [ShadowFunction(typeof(SourceEventStation))]
    public static partial class ShadowEventOnAbilityCasterShowUIMessage {
        [ShadowFunction]
        public static void OnAbilityCasted(Bodied attachedBodied, Ability ability) {
            if (ability.Tags.Contains(TagType.AbilityShowMessage)) {
                var followMessage = attachedBodied.GetComponent<FollowMessage>();
                followMessage.ShowFollowMessage(ability.Config.ViewName);
            }
        }
    }
}
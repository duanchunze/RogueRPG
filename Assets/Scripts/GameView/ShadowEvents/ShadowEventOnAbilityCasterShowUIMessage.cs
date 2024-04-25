namespace Hsenl.View {
    [ShadowFunction(typeof(EventStation))]
    public static partial class ShadowEventOnAbilityCasterShowUIMessage {
        [ShadowFunction]
        public static void OnAbilityCasted(Bodied attachedBodied, Ability ability) {
            if (ability.Tags.Contains(TagType.AbilityShowMessage)) {
                var followMessage = attachedBodied.GetComponent<HeadMessage>();
                followMessage.ShowFollowMessage(LocalizationHelper.GetAbilityLocalizationName(ability.Config));
            }
        }
    }
}
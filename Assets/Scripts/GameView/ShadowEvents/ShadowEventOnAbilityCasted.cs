namespace Hsenl.View {
    [ShadowFunction(typeof(EventStation))]
    public partial class ShadowEventOnAbilityCasted {
        [ShadowFunction]
        private static void OnAbilityCasted(Hsenl.Bodied attachedBodied, Hsenl.Ability ability) {
            if (ability.Tags.Contains(TagType.AbilityShowMessage)) {
                var followMessage = attachedBodied.GetComponent<HeadMessage>();
                followMessage.ShowFollowMessage(LocalizationHelper.GetAbilityLocalizationName(ability.Config));
            }
        }
    }
}
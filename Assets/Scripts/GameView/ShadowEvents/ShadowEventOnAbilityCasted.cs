namespace Hsenl.View {
    [ShadowFunction(typeof(SourceEventStation))]
    public partial class ShadowEventOnAbilityCasted {
        [ShadowFunction]
        private static void OnAbilityCasted(Hsenl.Bodied attachedBodied, Hsenl.Ability ability)
        {
            if (ability.Tags.Contains(TagType.AbilityShowMessage)) {
                var followMessage = attachedBodied.GetComponent<FollowMessage>();
                followMessage.ShowFollowMessage(ability.Config.ViewName);
            }
        }
    }
}
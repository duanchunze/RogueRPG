using Hsenl.EventType;

namespace Hsenl.View {
    public class EventOnAbilityCasterShowUIMessage : AEventSync<OnAbilityCasted> {
        protected override void Handle(OnAbilityCasted arg) {
            if (arg.ability.Tags.Contains(TagType.AbilityShowMessage)) {
                var followMessage = arg.caster.GetComponent<FollowMessage>();
                followMessage.ShowFollowMessage(arg.ability.Config.ViewName);
            }
        }
    }
}
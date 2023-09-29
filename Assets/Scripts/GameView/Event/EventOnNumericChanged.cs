using Hsenl.EventType;

namespace Hsenl.View {
    public class EventOnNumericChanged : AEventSync<OnNumericChanged> {
        protected override void Handle(OnNumericChanged arg) {
            var numerator = arg.numerator;
            switch ((NumericType)arg.numType) {
                case NumericType.Hp: {
                    var headMessage = numerator.Substantive.GetComponent<HeadMessage>();
                    if (headMessage != null) {
                        var max = numerator.GetValue(NumericType.MaxHp);
                        max.ToFloat();
                        headMessage.Enable = arg.now != 0;
                        headMessage.UpdateHp(arg.now / max);
                    }

                    break;
                }

                // case NumericType.Energy: {
                //     var headMessage = numerator.Substantive.GetComponent<HeadMessage>();
                //     if (headMessage != null) {
                //         var max = numerator.GetValue(NumericType.MaxEnergy);
                //         max.ToFloat();
                //         headMessage.UpdateEnergy(arg.now / max);
                //     }
                //
                //     break;
                // }

                case NumericType.Mana: {
                    var headMessage = numerator.Substantive.GetComponent<HeadMessage>();
                    if (headMessage != null) {
                        var max = numerator.GetValue(NumericType.MaxMana);
                        max.ToFloat();
                        headMessage.UpdateMana(arg.now / max);
                    }

                    break;
                }

                case NumericType.Height: {
                    var headMessage = numerator.Substantive.GetComponent<HeadMessage>();
                    if (headMessage != null) {
                        headMessage.UpdateFollowHeight(arg.now);
                    }

                    var followMessage = numerator.Substantive.GetComponent<FollowMessage>();
                    if (followMessage != null) {
                        followMessage.UpdateFollowHeight(arg.now + 1.1f);
                    }

                    break;
                }
            }
        }
    }
}
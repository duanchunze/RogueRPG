using Hsenl.EventType;

namespace Hsenl.View {
    public class EventOnNumericChanged : AEventSync<OnNumericChanged> {
        protected override void Handle(OnNumericChanged arg) {
            // 当人物的数值发生变化时, 对应的view层做出什么表现
            var numerator = arg.numerator;
            var bodied = numerator.Bodied;
            if (bodied is not Actor)
                return;

            switch ((NumericType)arg.numType) {
                case NumericType.Hp: {
                    var headMessage = bodied.GetComponent<HeadInfo>();
                    if (headMessage != null) {
                        var max = numerator.GetValue(NumericType.MaxHp);
                        max.ToFloat();
                        headMessage.Enable = arg.now != 0;
                        headMessage.UpdateHp(arg.now / max);
                    }

                    break;
                }

                case NumericType.Mana: {
                    var headMessage = bodied.GetComponent<HeadInfo>();
                    if (headMessage != null) {
                        var max = numerator.GetValue(NumericType.MaxMana);
                        max.ToFloat();
                        headMessage.UpdateMana(arg.now / max);
                    }

                    break;
                }

                case NumericType.Height: {
                    var headMessage = bodied.GetComponent<HeadInfo>();
                    if (headMessage != null) {
                        headMessage.UpdateFollowHeight(arg.now);
                    }

                    var followMessage = bodied.GetComponent<HeadMessage>();
                    if (followMessage != null) {
                        followMessage.UpdateFollowHeight(arg.now + 1.1f);
                    }

                    break;
                }
            }
        }
    }
}
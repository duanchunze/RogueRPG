using Hsenl.EventType;

namespace Hsenl {
    namespace EventType {
        public struct OnLocalizationChanged { }
    }

    public class Localization : Singleton<Localization> {
        public string Value { get; private set; }

        public void ChangeValue(string value) {
            if (this.Value == value)
                return;

            this.Value = value;
            EventSystem.Publish(new OnLocalizationChanged());
        }
    }
}
using System;
using System.Collections.Generic;

namespace Hsenl {
    [FrameworkMember]
    public abstract class UIEvent {
        private static readonly MultiList<Type, UIEvent> events = new(); // key: ui type, value: events
        private static readonly List<UIEvent> Empty = new(0);

        [OnEventSystemInitialized]
        private static void CacheEvents() {
            events.Clear();
            foreach (var type in EventSystem.GetTypesOfAttribute(typeof(UIEventAttribute))) {
                var evt = (UIEvent)Activator.CreateInstance(type);
                var uiType = type.BaseType?.GetGenericArguments()[0];
                events.Add(uiType, evt);
            }
        }

        internal static List<UIEvent> GetUIEventsOfUIType<T>() where T : UI<T> {
            var type = typeof(T);
            if (!events.TryGetValue(type, out var list)) {
                list = Empty;
            }

            return list;
        }
    }

    [UIEvent]
    public abstract class UIEvent<T> : UIEvent where T : UI<T> {
        internal static void InternalOnCreate(UI<T> ui) {
            foreach (var evt in GetUIEventsOfUIType<T>()) {
                ((UIEvent<T>)evt).OnCreate((T)ui);
            }
        }

        internal static void InternalOnOpen(UI<T> ui) {
            foreach (var evt in GetUIEventsOfUIType<T>()) {
                ((UIEvent<T>)evt).OnOpen((T)ui);
            }
        }

        internal static void InternalOnClose(UI<T> ui) {
            foreach (var evt in GetUIEventsOfUIType<T>()) {
                ((UIEvent<T>)evt).OnClose((T)ui);
            }
        }

        protected abstract void OnCreate(T ui);

        protected abstract void OnOpen(T ui);

        protected abstract void OnClose(T ui);
    }
}
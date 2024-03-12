using System;

namespace Hsenl {
    // 此处相当于是定义了所有事件源函数, 类比事件系统的定义 EventType
    [ShadowFunction(allowMultiShadowFuncs: true)]
    public static partial class SourceEventStation {
        [ShadowFunction]
        public static void OnCardResidenceChanged(ICardResidence residence) {
            OnCardResidenceChangedShadow(residence);
        }

        [ShadowFunction]
        public static void OnGoldNumberUpdate(int goldNum) {
            OnGoldNumberUpdateShadow(goldNum);
        }

        [ShadowFunction]
        public static void OnAbilityCasted(Bodied attachedBodied, Ability ability) {
            OnAbilityCastedShadow(attachedBodied, ability);
        }

        [ShadowFunction]
        public static void OnAbilityCooldown(Ability ability, float cooltime, float tilltime) {
            OnAbilityCooldownShadow(ability, cooltime, tilltime);
        }

        [ShadowFunction]
        public static void OnHeroUnlockUpdate() {
            OnHeroUnlockUpdateShadow();
        }
    }
}
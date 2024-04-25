using System;

namespace Hsenl {
    // 此处相当于是定义了所有事件源函数, 类比事件系统的定义 EventType
    [ShadowFunction(allowMultiShadowFuncs: true)]
    public static partial class EventStation {
        // 当卡牌所在地发生变化
        [ShadowFunction]
        public static void OnCardResidenceChanged(ICardResidence residence) {
            OnCardResidenceChangedShadow(residence);
        }

        // 当金币数量发生变化
        [ShadowFunction]
        public static void OnGoldNumberUpdate(int goldNum) {
            OnGoldNumberUpdateShadow(goldNum);
        }

        // 当技能被释放
        [ShadowFunction]
        public static void OnAbilityCasted(Bodied attachedBodied, Ability ability) {
            OnAbilityCastedShadow(attachedBodied, ability);
        }

        // 当技能进入cd
        [ShadowFunction]
        public static void OnAbilityCooldown(Ability ability, float cooltime, float tilltime) {
            OnAbilityCooldownShadow(ability, cooltime, tilltime);
        }

        // 当英雄解锁更新
        [ShadowFunction]
        public static void OnHeroUnlockUpdate() {
            OnHeroUnlockUpdateShadow();
        }
    }
}
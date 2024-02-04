using System;

namespace Hsenl.EventType {
    // 移动卡牌, 如实的表达移往何地, 其所代表的意义不管
    public struct MoveCard {
        /// <summary>
        /// 移到哪里去, 目的地和 slotInstanceId 至少得给一个, 不然不知道目标在哪
        /// </summary>
        public Type destination;

        /// <summary>
        /// 卡牌实例id
        /// </summary>
        public int cardInstanceId;

        /// <summary>
        /// 目标槽实例id, -1代表不指定某个具体的槽, 只要能装填就行
        /// </summary>
        public int slotInstanceId;

        /// <summary>
        /// 移动时, 是移动本体还是移动拷贝体
        /// </summary>
        public bool copyNew;
    }

    public struct FreezeCard {
        public int cardInstanceId;
    }

    // 当card residence发生了改变
    public struct OnCardResidenceChanged {
        public ICardResidence residence;
    }

    public struct OnHeadCardEquip {
        public CardBar cardBar;
        public CardBarHeadSlot slot;
        public Card card;
    }

    public struct OnHeadCardUnequip {
        public CardBar cardBar;
        public CardBarHeadSlot slot;
        public Card card;
    }

    public struct OnAssistCardEquip {
        public CardBar cardBar;
        public CardBarAssistSlot slot;
        public Card card;
    }

    public struct OnAssistCardUnequip {
        public CardBar cardBar;
        public CardBarAssistSlot slot;
        public Card card;
    }

    public struct RefreshStoreCards { }

    public struct ChangeAbilityAutoTrigger {
        public int cardInstanceId;
        public int cardBarSlotInstanceId;
    }

    public struct OnGoldNumberUpdate {
        public int goldNum;
    }

    public struct OnAbilityCasted {
        public Bodied attachedBodied;
        public Ability ability;
    }

    public struct OnAbilityCooldown {
        public Ability ability;
        public float cooltime;
        public float cooltilltime;
    }

    // 当英雄解锁情况发生更新
    public struct OnHeroUnlockUpdate { }
}
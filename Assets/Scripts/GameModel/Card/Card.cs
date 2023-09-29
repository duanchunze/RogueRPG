using System;
using Hsenl.card;
using MemoryPack;
using UnityEngine;

namespace Hsenl {
    // 卡牌可以包裹技能、状态、装备等等, 被包裹的技能就可以被当做卡牌来使用
    [Serializable]
    [MemoryPackable(GenerateType.CircularReference)]
    public partial class Card : Substantive {
        [MemoryPackOrder(50)]
        [MemoryPackInclude]
        public int cardId;

        [MemoryPackIgnore]
        public CardConfig Config => Tables.Instance.TbCardConfig.GetById(this.cardId);

        [MemoryPackOrder(51)]
        [MemoryPackInclude]
        public Bitlist cardType = new();

        // 看, 这里使用了循环引用, 实例化之后, 该字段引用的和Entity中Components里的是一个引用
        [MemoryPackOrder(52)]
        [MemoryPackInclude]
        public Substantive Source { get; private set; }

        [MemoryPackIgnore]
        public CardSlot StaySlot => this.GetParentSubstantiveAs<CardSlot>();

        protected override void OnDeserializedOverall() {
            this.Source.Link(this);
        }

        public void Wrap(Substantive src) {
            switch (src) {
                case Ability ability: {
                    this.Source = ability;
                    ability.SetParent(this.Entity);
                    ability.transform.NormalTransfrom();
                    break;
                }

                case AbilityAssist abilityAssist: {
                    this.Source = abilityAssist;
                    abilityAssist.SetParent(this.Entity);
                    abilityAssist.transform.NormalTransfrom();
                    break;
                }
            }

            src.Link(this);
        }

        public void Reset() {
            switch (this.Source) {
                case Ability ability: {
                    ability.SetParent(this.Entity);
                    ability.transform.NormalTransfrom();
                    break;
                }

                case AbilityAssist abilityAssist: {
                    abilityAssist.SetParent(this.Entity);
                    abilityAssist.transform.NormalTransfrom();
                    break;
                }
            }
        }
    }
}
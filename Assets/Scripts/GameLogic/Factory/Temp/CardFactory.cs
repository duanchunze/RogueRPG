using System;
using Hsenl.card;

namespace Hsenl {
    public static class CardFactory {
        public static Card Create(int cardId) {
            var config = Tables.Instance.TbCardConfig.GetById(cardId);
            return Create(config);
        }

        public static Card Create(string cardAlias) {
            var config = Tables.Instance.TbCardConfig.GetByAlias(cardAlias);
            return Create(config);
        }

        public static Card Create(CardConfig config) {
            switch (config.Wrappage) {
                case WrappageAbilityInfo wrappageAbilityInfo: {
                    var entity = Entity.Create(config.Alias);
                    var card = entity.AddComponent<Card>();
                    card.cardId = config.Id;
                    card.cardType.Add(CardType.Ability);
                    card.Wrap(AbilityFactory.Create(wrappageAbilityInfo.Alias));

                    return card;
                }

                case WrappageAbilityAssistInfo wrappageAbilityAssistInfo: {
                    var abilityAssistConfig = Tables.Instance.TbAbilityAssistConfig.GetByAlias(wrappageAbilityAssistInfo.Alias);
                    var entity = Entity.Create(config.Alias);
                    var card = entity.AddComponent<Card>();
                    card.cardId = config.Id;
                    card.cardType.Add(CardType.AbilityAssist);
                    card.Wrap(AbilityAssistFactory.Create(abilityAssistConfig));
                    return card;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static Card Clone(Card card) {
            var entity = Entity.Create(card.Name);
            var cloneCard = entity.AddComponent<Card>();
            cloneCard.cardType.Add(card.cardType);
            return cloneCard;
        }
    }
}
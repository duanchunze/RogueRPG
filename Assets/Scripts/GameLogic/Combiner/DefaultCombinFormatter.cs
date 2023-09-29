using System;
using System.Collections.Generic;

namespace Hsenl {
    public class DefaultCombinFormatter : CombinFormatter {
        protected override MultiList<Type, Type> GetCrossCombinFormatters() {
            var extends = new MultiList<Type, Type>() {
                {
                    typeof(Ability), new List<Type>() {
                        typeof(AbilityBar),
                        typeof(Actor),
                    }
                },
                
                {
                    typeof(Status), new List<Type>() {
                        typeof(StatusBar),
                        typeof(Actor),
                    }
                },
                
                {
                    typeof(CardBarAssistSlot), new List<Type>() {
                        typeof(CardBarHeadSlot),
                        typeof(CardBar),
                    }
                },
            };

            return extends;
        }
    }
}
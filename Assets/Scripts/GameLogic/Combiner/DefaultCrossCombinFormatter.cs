using System;
using System.Collections.Generic;

namespace Hsenl {
    public class DefaultCrossCombinFormatter : CrossCombinFormatter {
        protected override List<CrossCombinFormatterTypes> GetCrossCombinFormatterTypes() {
            var formatters = new List<CrossCombinFormatterTypes>() {
                new() {
                    childType = typeof(Ability),
                    layers = new CrossCombinFormatterTypes.Layer[] {
                        new() { // layer1
                            parentTypes = new Type[] { typeof(AbilityBar) },
                        },
                        new() { // layer2
                            parentTypes = new Type[] { typeof(Actor) },
                        }
                    },
                },

                new() {
                    childType = typeof(Status),
                    layers = new CrossCombinFormatterTypes.Layer[] {
                        new() { // layer1
                            parentTypes = new Type[] { typeof(StatusBar) },
                        },
                        new() { // layer2
                            parentTypes = new Type[] { typeof(Actor) },
                        }
                    },
                },

                new() {
                    childType = typeof(CardBarAssistSlot),
                    layers = new CrossCombinFormatterTypes.Layer[] {
                        new() { // layer1
                            parentTypes = new Type[] { typeof(CardBarHeadSlot) },
                        },
                        new() { // layer2
                            parentTypes = new Type[] { typeof(CardBar) },
                        }
                    },
                },
            };

            return formatters;
        }
    }
}
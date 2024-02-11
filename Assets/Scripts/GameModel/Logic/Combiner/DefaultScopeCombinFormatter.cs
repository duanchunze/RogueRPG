using System;
using System.Collections.Generic;

namespace Hsenl {
    public class DefaultScopeCombinFormatter : ScopeCombinFormatter {
        protected override List<ScopeCrossCombinFormatterTypes> GetCrossCombinFormatterTypes() {
            var formatters = new List<ScopeCrossCombinFormatterTypes>() {
                new() {
                    scopeType = typeof(Ability),
                    layers = new ScopeCrossCombinFormatterTypes.Layer[] {
                        new() { // layer1
                            types = new Type[] { typeof(AbilityBar) },
                        },
                        new() { // layer2
                            types = new Type[] { typeof(Actor) },
                        }
                    },
                },

                new() {
                    scopeType = typeof(Status),
                    layers = new ScopeCrossCombinFormatterTypes.Layer[] {
                        new() { // layer1
                            types = new Type[] { typeof(StatusBar) },
                        },
                        new() { // layer2
                            types = new Type[] { typeof(Actor) },
                        }
                    },
                },

                new() {
                    scopeType = typeof(CardBarAssistSlot),
                    layers = new ScopeCrossCombinFormatterTypes.Layer[] {
                        new() { // layer1
                            types = new Type[] { typeof(CardBarHeadSlot) },
                        },
                        new() { // layer2
                            types = new Type[] { typeof(CardBar) },
                        }
                    },
                },
            };

            return formatters;
        }
    }
}
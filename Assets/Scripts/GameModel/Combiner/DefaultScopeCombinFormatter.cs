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
                            types = new[] { typeof(AbilitesBar) },
                        },
                        new() { // layer2
                            types = new[] { typeof(Actor) },
                        }
                    },
                },

                new() {
                    scopeType = typeof(AbilityTrait),
                    layers = new ScopeCrossCombinFormatterTypes.Layer[] {
                        new() { // layer1
                            types = new[] { typeof(Ability) },
                        },
                    },
                },

                new() {
                    scopeType = typeof(AbilityPatch),
                    layers = new ScopeCrossCombinFormatterTypes.Layer[] {
                        new() { // layer1
                            types = new[] { typeof(Ability) },
                        },
                    },
                },

                new() {
                    scopeType = typeof(Status),
                    layers = new ScopeCrossCombinFormatterTypes.Layer[] {
                        new() { // layer1
                            types = new[] { typeof(StatusBar) },
                        },
                        new() { // layer2
                            types = new[] { typeof(Actor) },
                        }
                    },
                },
                
                new() {
                    scopeType = typeof(Prop),
                    layers = new ScopeCrossCombinFormatterTypes.Layer[] {
                        new() { // layer1
                            types = new[] { typeof(PropBar) },
                        },
                        new() { // layer2
                            types = new[] { typeof(Actor) },
                        }
                    },
                },
            };

            return formatters;
        }
    }
}
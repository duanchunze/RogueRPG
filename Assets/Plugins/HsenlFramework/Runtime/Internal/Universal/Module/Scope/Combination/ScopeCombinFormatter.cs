using System;
using System.Collections.Generic;

namespace Hsenl {
    [FrameworkMember]
    public abstract class ScopeCombinFormatter {
        internal static Dictionary<int, ScopeFormatterInfoCollect> CrossCombinFormatterInfoCollects { get; } = new();

        internal static void Init() {
            CrossCombinFormatterInfoCollects.Clear();
            foreach (var type in AssemblyHelper.GetSubTypes(typeof(ScopeCombinFormatter), EventSystem.GetAssemblies())) {
                var combinFormatter = (ScopeCombinFormatter)Activator.CreateInstance(type);

                foreach (var formatterOption in combinFormatter.GetCrossCombinFormatterOptions()) {
                    var componentIndex = Entity.GetComponentIndex(formatterOption.scopeType);
                    if (!CrossCombinFormatterInfoCollects.TryGetValue(componentIndex, out var collect)) {
                        collect = new ScopeFormatterInfoCollect();
                        CrossCombinFormatterInfoCollects.Add(componentIndex, collect);
                    }

                    collect.infiniteMatching = formatterOption.infiniteMatching;
                }

                foreach (var formatterTypes in combinFormatter.GetCrossCombinFormatterTypes()) {
                    if (!formatterTypes.scopeType.IsSubclassOf(typeof(Scope))) {
                        throw new Exception($"cross formatter type must inherit from <Scope>'{formatterTypes.scopeType.Name}'");
                    }

                    var componentIndex = Entity.GetComponentIndex(formatterTypes.scopeType);
                    if (!CrossCombinFormatterInfoCollects.TryGetValue(componentIndex, out var collect)) {
                        collect = new ScopeFormatterInfoCollect();
                        CrossCombinFormatterInfoCollects.Add(componentIndex, collect);
                    }

                    var formatterInfo = new ScopeFormatterInfo();
                    foreach (var layerType in formatterTypes.layers) {
                        foreach (var parentType in layerType.types) {
                            if (!parentType.IsSubclassOf(typeof(Scope))) {
                                throw new Exception($"cross formatter type must inherit from <Scope>'{parentType.Name}'");
                            }
                        }

                        var layer = new ScopeFormatterLayerInfo {
                            typeCacher = Entity.CombineComponentType(layerType.types)
                        };

                        formatterInfo.layerInfos.Add(layer);
                    }

                    collect.formatterInfos.Add(formatterInfo);
                }
            }
        }

        protected virtual List<ScopeCrossCombinFormatterTypes> GetCrossCombinFormatterTypes() {
            return new List<ScopeCrossCombinFormatterTypes>();
        }

        protected virtual List<ScopeCrossCombinFormatterOption> GetCrossCombinFormatterOptions() {
            return new List<ScopeCrossCombinFormatterOption>();
        }
    }

    internal class ScopeFormatterInfoCollect {
        public bool infiniteMatching; // 无限制的匹配, 将不受formatter的限制, 一层层向上匹配, 直到父域为空

        // 根据所有的formatter来判断某一层是否可以进行匹配
        public readonly List<ScopeFormatterInfo> formatterInfos = new();
    }

    internal class ScopeFormatterInfo {
        public readonly List<ScopeFormatterLayerInfo> layerInfos = new();

        // 当前层的匹配是否暂时成功(之所是"暂时的"是因为每一层都要进行一次判断), 该字段会在具体使用时, 被频繁的修改, 可以理解为是一个临时变量
        public bool succ;
    }

    internal class ScopeFormatterLayerInfo {
        public ComponentTypeCacher typeCacher;
    }

    public class ScopeCrossCombinFormatterTypes {
        public Type scopeType;
        public Layer[] layers;

        public class Layer {
            public Type[] types;
        }
    }

    public class ScopeCrossCombinFormatterOption {
        public Type scopeType;

        // 无限制的匹配, 将不受layers的限制, 一层层向上匹配, 直到父域为空
        public bool infiniteMatching;
    }
}
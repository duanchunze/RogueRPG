using System;
using System.Collections.Generic;

namespace Hsenl {
    [FrameworkMember]
    public abstract class CrossCombinFormatter {
        internal static Dictionary<int, CrossCombinFormatterInfoCollect> CrossCombinFormatterInfoCollects { get; } = new();

        [OnEventSystemInitialized]
        private static void Init() {
            CrossCombinFormatterInfoCollects.Clear();
            foreach (var type in AssemblyHelper.GetSubTypes(typeof(CrossCombinFormatter), EventSystem.GetAssemblies())) {
                var combinFormatter = (CrossCombinFormatter)Activator.CreateInstance(type);

                foreach (var formatterOption in combinFormatter.GetCrossCombinFormatterOptions()) {
                    var componentIndex = Entity.GetComponentIndex(formatterOption.childType);
                    if (!CrossCombinFormatterInfoCollects.TryGetValue(componentIndex, out var collect)) {
                        collect = new CrossCombinFormatterInfoCollect();
                        CrossCombinFormatterInfoCollects.Add(componentIndex, collect);
                    }

                    collect.infiniteMatching = formatterOption.infiniteMatching;
                }

                foreach (var formatterTypes in combinFormatter.GetCrossCombinFormatterTypes()) {
                    if (!formatterTypes.childType.IsSubclassOf(typeof(Scope))) {
                        throw new Exception($"cross formatter type must inherit from <Scope>'{formatterTypes.childType.Name}'");
                    }

                    var componentIndex = Entity.GetComponentIndex(formatterTypes.childType);
                    if (!CrossCombinFormatterInfoCollects.TryGetValue(componentIndex, out var collect)) {
                        collect = new CrossCombinFormatterInfoCollect();
                        CrossCombinFormatterInfoCollects.Add(componentIndex, collect);
                    }

                    var formatterInfo = new CrossCombinFormatterInfo();
                    foreach (var layerType in formatterTypes.layers) {
                        foreach (var parentType in layerType.parentTypes) {
                            if (!parentType.IsSubclassOf(typeof(Scope))) {
                                throw new Exception($"cross formatter type must inherit from <Scope>'{parentType.Name}'");
                            }
                        }

                        var layer = new CrossCombinFormatterLayerInfo {
                            parentTypeCacher = Entity.CombineComponentType(layerType.parentTypes)
                        };

                        formatterInfo.layerInfos.Add(layer);
                    }

                    collect.formatterInfos.Add(formatterInfo);
                }
            }
        }

        protected virtual List<CrossCombinFormatterTypes> GetCrossCombinFormatterTypes() {
            return new List<CrossCombinFormatterTypes>();
        }

        protected virtual List<CrossCombinFormatterOption> GetCrossCombinFormatterOptions() {
            return new List<CrossCombinFormatterOption>();
        }
    }

    internal class CrossCombinFormatterInfoCollect {
        public bool infiniteMatching; // 无限制的匹配, 将不受formatter的限制, 一层层向上匹配, 直到父域为空

        // 根据所有的formatter来判断某一层是否可以进行匹配
        public readonly List<CrossCombinFormatterInfo> formatterInfos = new();
    }

    internal class CrossCombinFormatterInfo {
        public readonly List<CrossCombinFormatterLayerInfo> layerInfos = new();

        // 当前层的匹配是否暂时成功(之所是"暂时的"是因为每一层都要进行一次判断), 该字段会在具体使用时, 被频繁的修改, 可以理解为是一个临时变量
        public bool succ;
    }

    internal class CrossCombinFormatterLayerInfo {
        public ComponentTypeCacher parentTypeCacher;
    }

    public class CrossCombinFormatterTypes {
        public Type childType;
        public Layer[] layers;

        public class Layer {
            public Type[] parentTypes;
        }
    }

    public class CrossCombinFormatterOption {
        public Type childType;

        // 无限制的匹配, 将不受layers的限制, 一层层向上匹配, 直到父域为空
        public bool infiniteMatching;
    }
}
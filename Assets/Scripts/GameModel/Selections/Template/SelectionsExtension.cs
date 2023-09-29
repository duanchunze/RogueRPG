using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;

namespace Hsenl {
    public static class SelectionsExtension {
        // 球形范围内选择敌对的最近目标
        public static SelectionTarget SelectSphereNearestTarget(this Selector self, float radius, IReadOnlyBitlist constrainsTags = null,
            IReadOnlyBitlist exclusiveTags = null) {
            if (self.Enable == false) return null;
            var searcher = self.GetSearcher<SearcherOfOverlapSphere>();
            searcher.position = self.transform.Position;
            searcher.radius = radius;
            searcher.layerMask = Constant.BodyLayerIncludeMask;
            var filterOfTags = self.GetFilter<FilterOfTags>();
            filterOfTags.constrainsTags = constrainsTags;
            filterOfTags.exclusivesTags = exclusiveTags;
            var selector = self.GetSelector<SelectorOfNearest>();
            selector.position = searcher.position;
            searcher.Obsolesce();
            searcher.Search().Filter(filterOfTags).Select(selector);
            return selector.Target;
        }

        public static SelectionTarget SelectShpereAliveNearestTarget(this Selector self, float radius, IReadOnlyBitlist constrainsTags = null,
            IReadOnlyBitlist exclusiveTags = null) {
            if (self.Enable == false) return null;
            var searcher = self.GetSearcher<SearcherOfOverlapSphere>();
            searcher.position = self.transform.Position;
            searcher.radius = radius;
            searcher.layerMask = Constant.BodyLayerIncludeMask;
            var filterOfTags = self.GetFilter<FilterOfTags>();
            filterOfTags.constrainsTags = constrainsTags;
            filterOfTags.exclusivesTags = exclusiveTags;
            var filterOfAlive = self.GetFilter<FilterOfAlive>();
            var filterOfObstacles = self.GetFilter<FilterOfObstacles>();
            var selector = self.GetSelector<SelectorOfNearest>();
            selector.position = searcher.position;
            searcher.Obsolesce();
            searcher.Search().Filter(filterOfTags).Filter(filterOfAlive).Filter(filterOfObstacles).Select(selector);
            return selector.Target;
        }

        public static IReadOnlyList<SelectionTarget> SelectShpereAliveTargets(this Selector self, float radius, IReadOnlyBitlist constrainsTags = null,
            IReadOnlyBitlist exclusiveTags = null) {
            if (self.Enable == false) return null;
            var searcher = self.GetSearcher<SearcherOfOverlapSphere>();
            searcher.position = self.transform.Position;
            searcher.radius = radius;
            searcher.layerMask = Constant.BodyLayerIncludeMask;
            var filterOfTags = self.GetFilter<FilterOfTags>();
            filterOfTags.constrainsTags = constrainsTags;
            filterOfTags.exclusivesTags = exclusiveTags;
            var filterOfAlive = self.GetFilter<FilterOfAlive>();
            searcher.Obsolesce();
            searcher.Search().Filter(filterOfTags).Filter(filterOfAlive);
            return filterOfAlive.Targets;
        }

        // 球形范围内, 敌对的, 活着的, 最近的若干个目标
        public static IReadOnlyList<SelectionTarget> SelectShpereAliveNearestTargets(this Selector self, float radius, int count,
            IReadOnlyBitlist constrainsTags = null, IReadOnlyBitlist exclusiveTags = null) {
            if (self.Enable == false) return null;
            var searcher = self.GetSearcher<SearcherOfOverlapSphere>();
            searcher.position = self.transform.Position;
            searcher.radius = radius;
            searcher.layerMask = Constant.BodyLayerIncludeMask;
            var filterOfTags = self.GetFilter<FilterOfTags>();
            filterOfTags.constrainsTags = constrainsTags;
            filterOfTags.exclusivesTags = exclusiveTags;
            var filterOfAlive = self.GetFilter<FilterOfAlive>();
            var filterOfObstacles = self.GetFilter<FilterOfObstacles>();
            var selector = self.GetSelector<SelectorsOfNearest>();
            selector.position = searcher.position;
            selector.count = count;
            searcher.Obsolesce();
            searcher.Search().Filter(filterOfTags).Filter(filterOfAlive).Filter(filterOfObstacles).Select(selector);
            return selector.Targets;
        }

        // 球形范围内, 敌对的, 活着的, 最近的若干个目标
        public static int SelectShpereAliveNearestTargets(this Selector self, float radius, int count, IList<SelectionTarget> list,
            IReadOnlyBitlist constrainsTags = null, IReadOnlyBitlist exclusiveTags = null) {
            var targets = self.SelectShpereAliveNearestTargets(radius, count, constrainsTags, exclusiveTags);
            var len = targets.Count;
            if (len != 0) {
                for (var i = 0; i < len; i++) {
                    list.Add(targets[i]);
                }
            }

            return len;
        }
    }
}
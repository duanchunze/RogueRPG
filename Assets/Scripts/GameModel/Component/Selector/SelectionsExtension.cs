using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;

namespace Hsenl {
    public static class SelectionsExtension {
        public static ASelectionsSearcher SearcherSphereBody(this Selector self, float radius, int layerMask = 1 << Constant.ReceptorLayer) {
            var searcher = self.GetSearcher<SearcherOfOverlapSphere>();
            searcher.position = self.transform.Position;
            searcher.radius = radius;
            searcher.layerMask = layerMask;
            searcher.Search();
            return searcher;
        }

        public static ASelectionsFilter FilterTags(this ASelections self, IReadOnlyBitlist constrainsTags, IReadOnlyBitlist exclusiveTags) {
            var filter = self.selector.GetFilter<FilterOfTags>();
            filter.constrainsTags = constrainsTags;
            filter.exclusivesTags = exclusiveTags;
            self.Filter(filter);
            return filter;
        }
        
        public static ASelectionsFilter FilterAlive(this ASelections self) {
            var filter = self.selector.GetFilter<FilterOfAlive>();
            self.Filter(filter);
            return filter;
        }
        
        public static ASelectionsFilter FilterObstacles(this ASelections self) {
            var filter = self.selector.GetFilter<FilterOfObstacles>();
            self.Filter(filter);
            return filter;
        }

        public static ASelectionsSelect SelectNearest(this ASelections self) {
            var select = self.selector.GetSelect<SelectOfNearest>();
            select.position = self.selector.transform.Position;
            self.Select(select);
            return select;
        }
        
        public static ASelectionsSelect SelectNearests(this ASelections self, int count) {
            var select = self.selector.GetSelect<SelectOfNearests>();
            select.position = self.selector.transform.Position;
            select.count = count;
            self.Select(select);
            return select;
        }

        // // 球形范围内选择敌对的最近目标
        // public static SelectionTarget SelectSphereNearestTarget(this Selector self, float radius, IReadOnlyBitlist constrainsTags = null,
        //     IReadOnlyBitlist exclusiveTags = null) {
        //     if (self.Enable == false) return null;
        //     var searcher = self.GetSearcher<SearcherOfOverlapSphere>();
        //     searcher.position = self.transform.Position;
        //     searcher.radius = radius;
        //     searcher.layerMask = 1 << Constant.BodyLayer;
        //     var filterOfTags = self.GetFilter<FilterOfTags>();
        //     filterOfTags.constrainsTags = constrainsTags;
        //     filterOfTags.exclusivesTags = exclusiveTags;
        //     var selector = self.GetSelect<SelectOfNearest>();
        //     selector.position = searcher.position;
        //     searcher.Obsolesce();
        //     searcher.Search().Filter(filterOfTags).Select(selector);
        //     return selector.Target;
        // }
        //
        // public static SelectionTarget SelectShpereAliveNearestTarget(this Selector self, float radius, IReadOnlyBitlist constrainsTags = null,
        //     IReadOnlyBitlist exclusiveTags = null) {
        //     if (self.Enable == false) return null;
        //     var searcher = self.GetSearcher<SearcherOfOverlapSphere>();
        //     searcher.position = self.transform.Position;
        //     searcher.radius = radius;
        //     searcher.layerMask = 1 << Constant.BodyLayer;
        //     var filterOfTags = self.GetFilter<FilterOfTags>();
        //     filterOfTags.constrainsTags = constrainsTags;
        //     filterOfTags.exclusivesTags = exclusiveTags;
        //     var filterOfAlive = self.GetFilter<FilterOfAlive>();
        //     var filterOfObstacles = self.GetFilter<FilterOfObstacles>();
        //     var selector = self.GetSelect<SelectOfNearest>();
        //     selector.position = searcher.position;
        //     searcher.Obsolesce();
        //     searcher.Search().Filter(filterOfTags).Filter(filterOfAlive).Filter(filterOfObstacles).Select(selector);
        //     return selector.Target;
        // }
        //
        // public static IReadOnlyList<SelectionTarget> SelectShpereAliveTargets(this Selector self, float radius, IReadOnlyBitlist constrainsTags = null,
        //     IReadOnlyBitlist exclusiveTags = null) {
        //     if (self.Enable == false) return null;
        //     var searcher = self.GetSearcher<SearcherOfOverlapSphere>();
        //     searcher.position = self.transform.Position;
        //     searcher.radius = radius;
        //     searcher.layerMask = 1 << Constant.BodyLayer;
        //     var filterOfTags = self.GetFilter<FilterOfTags>();
        //     filterOfTags.constrainsTags = constrainsTags;
        //     filterOfTags.exclusivesTags = exclusiveTags;
        //     var filterOfAlive = self.GetFilter<FilterOfAlive>();
        //     searcher.Obsolesce();
        //     searcher.Search().Filter(filterOfTags).Filter(filterOfAlive);
        //     return filterOfAlive.Targets;
        // }
        //
        // // 球形范围内, 敌对的, 活着的, 最近的若干个目标
        // public static IReadOnlyList<SelectionTarget> SelectShpereAliveNearestTargets(this Selector self, float radius, int count,
        //     IReadOnlyBitlist constrainsTags = null, IReadOnlyBitlist exclusiveTags = null) {
        //     if (self.Enable == false) return null;
        //     var searcher = self.GetSearcher<SearcherOfOverlapSphere>();
        //     searcher.position = self.transform.Position;
        //     searcher.radius = radius;
        //     searcher.layerMask = 1 << Constant.BodyLayer;
        //     var filterOfTags = self.GetFilter<FilterOfTags>();
        //     filterOfTags.constrainsTags = constrainsTags;
        //     filterOfTags.exclusivesTags = exclusiveTags;
        //     var filterOfAlive = self.GetFilter<FilterOfAlive>();
        //     var filterOfObstacles = self.GetFilter<FilterOfObstacles>();
        //     var selector = self.GetSelect<SelectOfNearests>();
        //     selector.position = searcher.position;
        //     selector.count = count;
        //     searcher.Obsolesce();
        //     searcher.Search().Filter(filterOfTags).Filter(filterOfAlive).Filter(filterOfObstacles).Select(selector);
        //     return selector.Targets;
        // }
        //
        // // 球形范围内, 敌对的, 活着的, 最近的若干个目标
        // public static int SelectShpereAliveNearestTargets(this Selector self, float radius, int count, IList<SelectionTarget> list,
        //     IReadOnlyBitlist constrainsTags = null, IReadOnlyBitlist exclusiveTags = null) {
        //     var targets = self.SelectShpereAliveNearestTargets(radius, count, constrainsTags, exclusiveTags);
        //     var len = targets.Count;
        //     if (len != 0) {
        //         for (var i = 0; i < len; i++) {
        //             list.Add(targets[i]);
        //         }
        //     }
        //
        //     return len;
        // }
    }
}
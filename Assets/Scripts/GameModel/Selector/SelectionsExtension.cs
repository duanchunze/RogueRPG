using System;
using System.Collections.Generic;

namespace Hsenl {
    public static class SelectionsExtension {
        public static ASelectionsSearcher SearcherSphereBody(this SelectorDefault self, float radius, int layerMask = 1 << Constant.ReceptorLayer) {
            return self.SearcherSphereBody(self.transform.Position, radius, layerMask);
        }

        public static ASelectionsSearcher SearcherSphereBody(this SelectorDefault self, Vector3 position, float radius, int layerMask = 1 << Constant.ReceptorLayer) {
            var searcher = self.GetSearcher<SearcherOfOverlapSphere>();
            searcher.position = position;
            searcher.radius = radius;
            searcher.layerMask = layerMask;
            searcher.Search();
            return searcher;
        }

        public static ASelectionsSearcher SearcherSectorBody(this SelectorDefault self, float radius, float angle, int layerMask = 1 << Constant.ReceptorLayer) {
            return self.SearcherSectorBody(self.transform.Position, radius, self.transform.Forward, angle, layerMask);
        }

        public static ASelectionsSearcher SearcherSectorBody(this SelectorDefault self, Vector3 position, float radius, Vector3 dir, float angle,
            int layerMask = 1 << Constant.ReceptorLayer) {
            var searcher = self.GetSearcher<SearcherOfOverlapSector>();
            searcher.position = position;
            searcher.radius = radius;
            searcher.angle = angle;
            searcher.dir = dir;
            searcher.layerMask = layerMask;
            searcher.Search();
            return searcher;
        }

        public static ASelectionsFilter FilterTags(this ASelections self, IReadOnlyBitlist constrainsTags, IReadOnlyBitlist exclusiveTags) {
            var filter = self.Selector.GetFilter<FilterOfTags>();
            filter.constrainsTags = constrainsTags;
            filter.exclusivesTags = exclusiveTags;
            self.Filter(filter);
            return filter;
        }

        public static ASelectionsFilter FilterTargets(this ASelections self, Bodied target) {
            var filter = self.Selector.GetFilter<FilterOfTargets>();
            filter.targets ??= new();
            filter.targets.Clear();
            filter.targets.Add(target);
            self.Filter(filter);
            return filter;
        }

        public static ASelectionsFilter FilterTargets(this ASelections self, IList<Bodied> targets) {
            var filter = self.Selector.GetFilter<FilterOfTargets>();
            filter.targets ??= new();
            filter.targets.Clear();
            filter.targets.AddRange(targets);
            self.Filter(filter);
            return filter;
        }

        public static ASelectionsFilter FilterAlive(this ASelections self) {
            var filter = self.Selector.GetFilter<FilterOfAlive>();
            self.Filter(filter);
            return filter;
        }

        public static ASelectionsFilter FilterBackToSelf(this ASelections self) {
            var filter = self.Selector.GetFilter<FilterOfBackToSelf>();
            self.Filter(filter);
            return filter;
        }

        public static ASelectionsFilter FilterObstacles(this ASelections self) {
            var filter = self.Selector.GetFilter<FilterOfObstacles>();
            self.Filter(filter);
            return filter;
        }

        public static ASelectionsFilter FilterThreat(this ASelections self) {
            var filter = self.Selector.GetFilter<FilterOfThreat>();
            self.Filter(filter);
            return filter;
        }

        public static ASelectionsSelect SelectNearest(this ASelections self) {
            var select = self.Selector.GetSelect<SelectOfNearest>();
            select.position = self.Selector.transform.Position;
            self.Select(select);
            return select;
        }

        public static ASelectionsSelect SelectNearests(this ASelections self, int count) {
            var select = self.Selector.GetSelect<SelectOfNearests>();
            select.position = self.Selector.transform.Position;
            select.count = count;
            self.Select(select);
            return select;
        }
    }
}
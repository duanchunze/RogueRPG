﻿namespace Hsenl.CrossCombiner {
    public class Ability_Actor_Combiner : CrossCombiner<Ability, Actor> {
        // 这里为了省事, 就把 Numerator和ProcedureLine相关的组件交互都写在一个组合里面了, 
        // 这么做的话就需要确定在组合前, 相关组件都已经被添加过了.
        protected override void OnCombin(Ability arg1, Actor arg2) {
            var numeratorNode = arg1.GetComponent<NumericNode>();
            var numerator = arg2.GetComponent<Numerator>();
            if (numerator != null && numeratorNode != null)
                numerator.Attach(numeratorNode);

            var pln = arg1.GetComponent<ProcedureLineNode>();
            var pl = arg2.GetComponent<ProcedureLine>();
            if (pl != null && pln != null)
                pl.Attach(pln);
        }

        protected override void OnDecombin(Ability arg1, Actor arg2) {
            var numeratorNode = arg1.GetComponent<NumericNode>();
            var numerator = arg2.GetComponent<Numerator>();
            if (numerator != null && numeratorNode != null)
                numerator.Detach(numeratorNode);

            var pln = arg1.GetComponent<ProcedureLineNode>();
            var pl = arg2.GetComponent<ProcedureLine>();
            if (pl != null && pln != null)
                pl.Detach(pln);
        }
    }
}
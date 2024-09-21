namespace Hsenl {
    public enum PliHarmPriority {
        // 合法性判断
        Validity,
        // 丢失被选中: 每次造成伤害时, 会对所有选中自己为目标的selector, 丢失选中(可以让每次伤害都是偷袭)
        MissBeSelectOnHarming,
        // 分裂投射物
        SplitBolt,
        // 计算数值
        CalcNumeric,
        // 仲裁
        Arbiter,
        // 造成伤害
        TakeHurt,
        // 仇恨(给伤害的目标增加仇恨)
        Hatred,
        // 附加状态
        AdditionalStatus,
        AdditionalStatus2,
        // 吸血
        Vamp,
        // 恢复能量
        RecoverEnergy,
        // 触发释放(当伤害时, 当前worker所属的caster有概率开始施法)
        CastTrigger,
        
        // view表现(音效、特效等)
        ViewExpression,
    }
}
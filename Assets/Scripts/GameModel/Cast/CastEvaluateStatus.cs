namespace Hsenl {
    public enum CastEvaluateStatus {
        // 评估成功, 可以释放了
        Success,
        // 虽然没成功, 但是可以解决, 比如距离不够, 具体的怎么算可以解决, 由用户自己决定, 只要行为树执行的结果是running, 就是trying
        Trying,
        
        // 以下的就属于没成功, 且不能解决
        PriorityStateEnterFailure,
        PickTargetFailure,
        Cooldown,
        Mana,
        MoreThanMaxSummoningNum, // 超出最大召唤数
    }
}
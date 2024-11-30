namespace Hsenl {
    public enum CastEvaluateState {
        // 无效, 可能是方法没被实现, 或者其他未知错误
        Invalid,
        // 评估成功, 可以释放了
        Success,
        // 虽然没成功, 但施法器可以自行解决, 我们什么都不需要做, 我们也可以主动停止Try
        Trying,
        
        
        Cooldown,
        Mana,
        PriorityStateEnterFailure,

        
        // 没有指定目标
        NoTarget,
        // 目标阵营不符
        TargetFactionDiscrepancy,
        // 选中目标失败
        PickTargetFailure,
        // 距离不足
        DistanceDeficiency,
        // 角度不足
        AngeleDeficiency,
        // 有障碍物
        HasObstacles,
        // 不是背对自己
        NotBackForMe,
        
        
        // 超出最大召唤数
        MoreThanMaxSummoningNum,
    }
}
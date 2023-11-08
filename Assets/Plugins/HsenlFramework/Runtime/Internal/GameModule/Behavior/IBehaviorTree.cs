namespace Hsenl {
    public interface IBehaviorTree {
        Bodied Bodied { get; }
        Bodied Owner { get; }
        bool RealEnable { get; }
        IBlackboard Blackboard { get; }
        INode CurrentNode { get; internal set; }
        void SetEntryNode(INode node);
        NodeStatus Tick();
        void Reset();
        void Abort();
    }

    // 开发行为树的泛型版本是因为希望给节点细化分类
    // 例如一个技能的<增加CD的节点>, 和一个AI的<靠近敌人的节点>, 这两个节点即不相关, 也不通用, 但如果每次选择<靠近敌人的节点>的时候, 都能看到
    // <增加CD的节点>, 很糟心, 有了泛型, 就可以通过泛型区分, 技能类的节点和AI类节点
    // 同时也增加了拓展性
    // 使用 out 协变, 增加向下的兼容性
    public interface IBehaviorTree<out T> : IBehaviorTree where T : IBehaviorTree<T> {
        // 如非必要, 尽量不在泛型接口里声明函数, 转而在写在 IBehaviorTree 里, 尽量通用, 不要把继承关系卡的太死
    }
}
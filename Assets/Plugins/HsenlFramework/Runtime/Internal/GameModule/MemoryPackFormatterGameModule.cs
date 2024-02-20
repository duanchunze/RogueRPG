namespace Hsenl {
    public class MemoryPackFormatterGameModule : MemoryPackFormatter {
        protected override void Register() {
            RegisterUnion<IBlackboard>();
            RegisterUnion<INode>();
            RegisterUnion<INode<BehaviorTree>>();
            RegisterUnion<Node<BehaviorTree>>();

            RegisterUnion<IStageNode>();
            RegisterUnion<ITimeLine>();
            RegisterUnion<ITimeNode>();
            RegisterUnion<IProcedureLineWorker>();
            RegisterUnion<IRecord>();
        }
    }
}
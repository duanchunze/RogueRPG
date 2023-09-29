namespace Hsenl {
    /* 面包节点, 代表所有可以添加子节点的节点的统称
     */
    public interface IBreadNode { }

    public interface IBreadNode<in T> where T : INode {
        void AddChild(T t);
        void RemoveChild(T t);
        void Clear();
    }
}
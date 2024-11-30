using System;
using System.Threading.Tasks;

namespace Hsenl {
    // 冒险
    // 为什么要用剧本来实现?
    // 主要是为了借用行为树的那些start, open等事件, 不用再自己实现一遍了. 而且行为树本身就可以实现复杂的逻辑
    [Serializable]
    public class Adventure : Screenplay<IRecord, INode<Adventure>> {
        public int configId;
        public adventure.AdventureConfig Config => Tables.Instance.TbAdventureConfig.GetById(this.configId);
    }
}
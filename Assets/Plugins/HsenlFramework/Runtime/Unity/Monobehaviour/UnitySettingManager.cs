using System.Collections.Generic;
using UnityEngine;

namespace Hsenl {
    // 项目迁移的时候, 总会因为某些设置, 出现冲突, 或者干脆忘记某些设置, 这里会记录该项目的所有设置点
    public class UnitySettingManager : MonoBehaviour {
        public List<string> LayerNeeds = new() { };
    }
}
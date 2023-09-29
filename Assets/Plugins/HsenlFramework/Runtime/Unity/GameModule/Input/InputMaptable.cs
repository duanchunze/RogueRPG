using System;
using System.Collections.Generic;

namespace Hsenl {
    /* 输入映射表
     * 把设备的输入和游戏内的操作进行映射，比如键盘的空格键映射游戏中的跳跃
     * 设备输入是view层面的，即InputCode
     * 游戏内操作是model层面的，即ControlCode
     * 当view层发生输入时，由view层通过查询映射表，把设备输入转为游戏操作，然后发送消息到model层
     */
    public static class InputMaptable {
        private static readonly SortedMultiHashSet<InputCode, int> _table = new(); // key: _, value: ControlCode, 由我们自己定义

        public static void RegisterControlCode<T>(InputCode inputCode, T controlCode) where T : Enum {
            RegisterControlCode(inputCode, controlCode.GetHashCode());
        }

        public static void UnregisterControlCode<T>(InputCode inputCode, T controlCode) where T : Enum {
            UnregisterControlCode(inputCode, controlCode.GetHashCode());
        }

        public static void RegisterControlCode(InputCode inputCode, int controlCode) {
            _table.Add(inputCode, controlCode);
        }

        public static void UnregisterControlCode(InputCode inputCode, int controlCode) {
            _table.Remove(inputCode, controlCode);
        }

        public static HashSet<int> QueryControlCode(InputCode inputCode) {
            if (_table.TryGetValue(inputCode, out var result)) {
                return result;
            }

            return null;
        }
    }
}
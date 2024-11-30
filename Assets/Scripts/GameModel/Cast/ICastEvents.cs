namespace Hsenl {
    // 提供一些事件, 供有需要时使用, 比如有些复杂操作的技能需要 按键持续按下和抬起的事件
    public interface ICastEvents {
        void CastStart();
        void CastEnd();
    }
}
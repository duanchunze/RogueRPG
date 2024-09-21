using System;

namespace Hsenl {
    [Serializable]
    public class Minion : Unbodied {
        public Bodied master; // 主人
        public Bodied source; // 来源(具体是谁把他创造出来的, 技能?道具?)
        public event Action<Minion> onOver;

        public void OnOver() {
            try {
                this.onOver?.Invoke(this);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        protected override void OnDestroy() {
            this.OnOver();
        }
    }
}
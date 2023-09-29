namespace Hsenl {
    public abstract class SelectionsObsolete {
        public Selector selector;
        
        protected int obsoleteTillFrame;
        public int obsoleteFrame = 1;
        protected bool isUpdate;

        public bool IsObsolete {
            get {
                this.isUpdate = TimeInfo.FrameCount > this.obsoleteTillFrame;
                return this.isUpdate;
            }
        }

        public void RecalculateObsoleteFrame() {
            this.obsoleteTillFrame = TimeInfo.FrameCount + this.obsoleteFrame;
        }
        
        public void Obsolesce() {
            this.obsoleteTillFrame = -1;
        }
    }
}
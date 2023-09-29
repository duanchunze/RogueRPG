namespace Hsenl {
    public struct SinglePass {
        public bool prev;
        public bool curr;

        public SinglePass(bool enable) {
            this.prev = enable;
            this.curr = enable;
        }

        public void Set(bool enable) {
            this.curr = enable;
        }

        public bool Check() {
            return this.curr && this.curr != this.prev;
        }

        public void Run() {
            this.prev = this.curr;
        }

        public void Reset(bool enable = false) {
            this.prev = enable;
            this.curr = enable;
        }
    }
}
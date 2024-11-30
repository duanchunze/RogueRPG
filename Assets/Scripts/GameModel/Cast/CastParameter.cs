namespace Hsenl {
    public class CastParameter {
        public SelectionTargetDefault target;
        public Vector3? vector3Value;

        public void Reset() {
            this.target = null;
            this.vector3Value = null;
        }
    }
}
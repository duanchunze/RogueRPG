namespace Hsenl {
    /// <summary>
    /// 包围盒
    /// </summary>
    public struct FixBoundingBox {
        public Vector3 min;
        public Vector3 max;
        public Vector3 center;

        public Vector3 axisX;
        public Vector3 axisY;
        public Vector3 axisZ;

        public FixBoundingBox(Vector3 min, Vector3 max) {
            this.min = min;
            this.max = max;
            this.center = (max - min) * Fixp.Half;

            this.axisX = Vector3.Right;
            this.axisY = Vector3.Up;
            this.axisZ = Vector3.Forward;
        }

        public override string ToString() {
            return $"{min} | {max}";
        }
    }
}
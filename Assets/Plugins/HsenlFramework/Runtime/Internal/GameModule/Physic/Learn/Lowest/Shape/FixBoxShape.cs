#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    public class FixBoxShape : FixShape {
        public override FittingDegreeType fittingDegreeType => FittingDegreeType.Perfect;

        public FixBoxShape(FLOAT xyz) {
            var size = new Vector3(xyz);
            Init(ref size);
        }

        public FixBoxShape(Vector3 size) {
            Init(ref size);
        }

        public FixBoxShape(FLOAT length, FLOAT width, FLOAT height) {
            var size = new Vector3(length, height, width);
            Init(ref size);
        }

        public FixBoxShape(Vector3 center, Vector3 size) {
            Init(ref center, ref size);
        }

        public void Init(ref Vector3 size) {
            Vector3.Multiply(ref size, Fixp.Half, out var halfSize);
            this._boundingBox = new AABB(-halfSize, halfSize);
            this._box = new Box(Vector3.Zero, Vector3.Right, Vector3.Up, Vector3.Forward, halfSize);
        }

        public void Init(ref Vector3 center, ref Vector3 size) {
            Vector3.Multiply(ref size, Fixp.Half, out var halfSize);
            Vector3.Subtract(ref center, ref halfSize, out var min);
            Vector3.Add(ref halfSize, ref center, out var max);
            this._boundingBox = new AABB(min, max);
            this._box = new Box(center, Vector3.Right, Vector3.Up, Vector3.Forward, halfSize);
        }

        public override void SupportPoint(ref Vector3 direction, out Vector3 point) {
            // var signX = FixMath.Abs(direction.x) < Fixp.Epsilon ? 0 : FixMath.Sign(direction.x);
            // var signY = FixMath.Abs(direction.y) < Fixp.Epsilon ? 0 : FixMath.Sign(direction.y);
            // var signZ = FixMath.Abs(direction.z) < Fixp.Epsilon ? 0 : FixMath.Sign(direction.z);
            // point.x = signX * _box.extents.x;
            // point.y = signY * _box.extents.y;
            // point.z = signZ * _box.extents.z;
            point.x = Math.Sign(direction.x) * _box.extents.x;
            point.y = Math.Sign(direction.y) * _box.extents.y;
            point.z = Math.Sign(direction.z) * _box.extents.z;
        }
    }
}
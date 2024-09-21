using System;

namespace Hsenl {
    /// <summary>
    /// 碰撞者
    /// <para>核心是shape</para>>
    /// <para>自己掌管诸如旋转、缩放、位置等信息</para>>
    /// </summary>
    public abstract class FixCollider : IBroadPhase, INarrowPhase {
        internal Matrix3x3 _orientation = Matrix3x3.Identity;
        internal Matrix3x3 _invOrientation = Matrix3x3.Identity;
        internal Vector3 _position;
        internal Quaternion _rotation;
        internal Vector3 _scale;

        internal AABB _boundingBox;
        internal Box _box;

        internal bool _isActived;
        internal bool _isStatic;
        internal bool _isTrigger;

        private FixRigidbody m_rigidbody;
        private FixShape m_shape;

        public Matrix3x3 orientation => _orientation;
        public Matrix3x3 invOrientation => _invOrientation;
        public Vector3 position => _position;

        public event Action<FixCollisionInfo> onCollisionStay;

        /// <summary>
        /// 是否是最新的包围盒
        /// </summary>
        private bool m_isLatestBoundingBox = false;

        /// <summary>
        /// 是否是最新的盒体
        /// </summary>
        private bool m_isLatestBox = false;

        /// <summary>
        /// 刚体
        /// </summary>
        public FixRigidbody rigidbody => m_rigidbody;

        /// <summary>
        /// 形状
        /// </summary>
        public FixShape shape => m_shape;

        public AABB boundingBox => _boundingBox;

        public Box box => _box;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="shape"></param>
        public FixCollider(FixShape shape) {
            this.m_shape = shape;
            this._boundingBox = shape._boundingBox;
            this._box = shape._box;
        }

        private Vector3 supportLocalDirection;

        public void SupportPoint(ref Vector3 direction, out Vector3 point) {
            // 使用逆矩阵，把世界方向，转成support坐标系的局部方向
            Matrix3x3.Transform(ref _invOrientation, ref direction, out supportLocalDirection);
            // 求出该形状的该方向的最远的点（形状坐标系）
            this.m_shape.SupportPoint(ref supportLocalDirection, out point);
            // 再把point转成世界坐标
            Matrix3x3.Transform(ref _orientation, ref point, out point);
            // 加上位移
            Vector3.Add(ref point, ref _position, out point);
        }

        public void SupportCenter(out Vector3 center) {
            Vector3.Add(ref this.m_shape._geometricCenter, ref _position, out center);
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update() {
            m_isLatestBoundingBox = false;
            m_isLatestBox = false;
        }

        /// <summary>
        /// 更新包围盒
        /// <para>从shape处取出最原始的包围盒，然后进行旋转、缩放、位移的处理</para>>
        /// </summary>
        public void UpdateBoundingBox() {
            if (m_isLatestBoundingBox) {
                return;
            }

            m_isLatestBoundingBox = true;
            {
                if (_orientation != Matrix3x3.Identity) {
                    Matrix3x3.Transform(ref _orientation, ref this.m_shape._boundingBox.min, out _boundingBox.min);
                    Matrix3x3.Transform(ref _orientation, ref this.m_shape._boundingBox.max, out _boundingBox.max);
                }

                _boundingBox.min = this.m_shape._boundingBox.min + _position;
                _boundingBox.max = this.m_shape._boundingBox.max + _position;
            }
        }

        /// <summary>
        /// 更新盒体
        /// </summary>
        public void UpdateBox() {
            if (m_isLatestBox) {
                return;
            }

            m_isLatestBox = true;
            {
                if (_orientation != Matrix3x3.Identity) {
                    Matrix3x3.Transform(ref _orientation, ref this.m_shape._box.axis0, out this._box.axis0);
                    Matrix3x3.Transform(ref _orientation, ref this.m_shape._box.axis1, out this._box.axis1);
                    Matrix3x3.Transform(ref _orientation, ref this.m_shape._box.axis2, out this._box.axis2);
                }

                Vector3.Add(ref this.m_shape._box.center, ref _position, out this._box.center);
                Vector3.Scale(ref this.m_shape._box.extents, ref _scale, out this._box.extents);
            }
        }

        internal void Internal_OnCollisionStay(FixCollisionInfo other) {
            InvokeOnCollisionStay(ref other);
        }

        private void InvokeOnCollisionStay(ref FixCollisionInfo other) {
            try {
                onCollisionStay?.Invoke(other);
            }
            catch (Exception e) {
                throw new Exception(e.ToString());
            }
        }
    }
}
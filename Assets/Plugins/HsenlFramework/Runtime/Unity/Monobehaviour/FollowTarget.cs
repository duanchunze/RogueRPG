using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Hsenl {
    public class FollowTarget : MonoBehaviour {
        /// <summary>
        /// 跟随的目标
        /// </summary>
        public UnityEngine.Transform targetTransform;

        /// <summary>
        /// 始终看向目标
        /// </summary>
        public bool useLookAt;

        [Header("参数")]
        public float smoothFollow;

        [FormerlySerializedAs("offset")]
        public Vector3 _positionOffset;

        private void Update() {
            if (this.targetTransform) {
                var position = this.transform.position;
                var targetPosition = this.targetTransform.position;
                var desiredPosition = targetPosition + this._positionOffset;
                if (this.smoothFollow > 0) {
                    var smoothedPosition = Vector3.Lerp(position, desiredPosition, this.smoothFollow * TimeInfo.DeltaTime);
                    this.transform.position = smoothedPosition;
                }
                else {
                    this.transform.position = desiredPosition;
                }

                if (this.useLookAt) {
                    var forward = targetPosition - desiredPosition;
                    var rotation = quaternion.LookRotation(forward.normalized, math.up());
                    this.transform.rotation = rotation;
                }
            }
        }

        public void FollowImmediately(Vector3 pos) {
            this.transform.position = pos + this._positionOffset;
        }

        public void RecalculateOffset() {
            if (this.targetTransform) {
                this._positionOffset = this.transform.position - this.targetTransform.position;
            }
        }
    }
}
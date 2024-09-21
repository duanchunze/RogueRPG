using UnityEngine;

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

        public UnityEngine.Vector3 positionOffset;

        public UnityEngine.Vector3 rotationOffset;

        private void Update() {
            if (this.targetTransform) {
                var position = this.transform.position;
                var targetPosition = this.targetTransform.position;
                var desiredPosition = targetPosition + this.positionOffset;
                if (this.smoothFollow > 0) {
                    var smoothedPosition = UnityEngine.Vector3.Lerp(position, desiredPosition, this.smoothFollow * TimeInfo.DeltaTime);
                    this.transform.position = smoothedPosition;
                }
                else {
                    this.transform.position = desiredPosition;
                }

                if (this.useLookAt) {
                    var forward = targetPosition - desiredPosition;
                    var rota = Quaternion.Euler(this.rotationOffset);
                    forward *= rota;
                    var rotation = Quaternion.CreateLookRotation(forward, Vector3.Up);
                    this.transform.rotation = rotation;
                }
            }
        }

        public void FollowImmediately(UnityEngine.Vector3 pos) {
            this.transform.position = pos + this.positionOffset;
        }

        public void RecalculateOffset() {
            if (this.targetTransform) {
                this.positionOffset = this.transform.position - this.targetTransform.position;
            }
        }
    }
}
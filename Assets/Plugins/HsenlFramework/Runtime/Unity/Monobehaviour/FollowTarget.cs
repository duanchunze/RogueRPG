using FixedMath;
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

        public Vector3 positionOffset;

        public Vector3 rotationOffset;

        private void Update() {
            if (this.targetTransform) {
                var position = this.transform.position;
                var targetPosition = this.targetTransform.position;
                var desiredPosition = targetPosition + this.positionOffset;
                if (this.smoothFollow > 0) {
                    var smoothedPosition = Vector3.Lerp(position, desiredPosition, this.smoothFollow * TimeInfo.DeltaTime);
                    this.transform.position = smoothedPosition;
                }
                else {
                    this.transform.position = desiredPosition;
                }

                if (this.useLookAt) {
                    var forward = (targetPosition - desiredPosition).ToFVector3();
                    var rota = FQuaternion.Euler(this.rotationOffset.ToFVector3());
                    forward *= rota;
                    var rotation = FQuaternion.CreateLookRotation(forward, FVector3.Up);
                    this.transform.rotation = rotation.ToUnityQuaternion();
                }
            }
        }

        public void FollowImmediately(Vector3 pos) {
            this.transform.position = pos + this.positionOffset;
        }

        public void RecalculateOffset() {
            if (this.targetTransform) {
                this.positionOffset = this.transform.position - this.targetTransform.position;
            }
        }
    }
}
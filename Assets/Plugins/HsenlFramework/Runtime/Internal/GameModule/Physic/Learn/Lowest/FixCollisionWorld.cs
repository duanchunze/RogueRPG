using System;
using System.Collections.Generic;
using System.Drawing;
using FixedMath;
using UnityEngine;

#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    internal class FixCollisionWorld {
        internal List<FixCollider> _colliders = new List<FixCollider>();

        internal bool AddCollider(FixCollider collider) {
            if (_colliders.Contains(collider)) {
                return false;
            }

            _colliders.Add(collider);
            return true;
        }

        internal bool RemoveBody(FixCollider collider) {
            return _colliders.Remove(collider);
        }

        /// <summary>
        /// 检测整个世界的碰撞体碰撞
        /// </summary>
        internal void DetectWorld() {
            int count = _colliders.Count;
            for (int i = 0; i < count; i++) {
                for (int j = i + 1; j < count; j++) {
                    var col1 = _colliders[i];
                    var col2 = _colliders[j];

                    if (CollisionDetection.ShapeCollision(col1, col2, out var normal, out var point, out var penetration)) {
                        col1.Internal_OnCollisionStay(new FixCollisionInfo {
                            collider = col2, point = point, normal = normal, penetration = penetration
                        });
                        col2.Internal_OnCollisionStay(new FixCollisionInfo {
                            collider = col1, point = point, normal = normal, penetration = penetration
                        });
                    }
                }
            }
        }

        /// <summary>
        /// 射线与整个碰撞世界做检测
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        /// <param name="detectPrecisionType"></param>
        /// <param name="hit"></param>
        /// <returns></returns>
        internal bool RaycastToWorld(ref FVector3 origin, ref FVector3 direction,
            DetectPrecisionType detectPrecisionType, out FixRaycastHit hit) {
            switch (detectPrecisionType) {
                case DetectPrecisionType.Rough:
                    hit = null;
                    return RaycastToWorldRough(ref origin, ref direction);

                case DetectPrecisionType.TryFast:
                    hit = null;
                    return RaycastToWorldTryFast(ref origin, ref direction);

                case DetectPrecisionType.Accurate:
                    return RaycastToWorldAccurate(ref origin, ref direction, out hit);
            }

            bool RaycastToWorldRough(ref FVector3 l_origin, ref FVector3 l_direction) {
                foreach (var collider in _colliders) {
                    if (RaycastToColliderRough(ref l_origin, ref l_direction, collider)) {
                        return true;
                    }
                }

                return false;
            }

            bool RaycastToWorldTryFast(ref FVector3 l_origin, ref FVector3 l_direction) {
                foreach (var collider in _colliders) {
                    if (RaycastToColliderTryFast(ref l_origin, ref l_direction, collider)) {
                        return true;
                    }
                }

                return false;
            }

            bool RaycastToWorldAccurate(ref FVector3 l_origin, ref FVector3 l_direction, out FixRaycastHit l_hit) {
                foreach (var collider in _colliders) {
                    if (!RaycastToColliderAccurate(ref l_origin, ref l_direction, collider, out var normal,
                            out var fraction)) {
                        continue;
                    }

                    l_hit = new FixRaycastHit() {
                        collider = collider,
                        fraction = fraction,
                        normal = normal,
                        origin = l_origin,
                        direction = l_direction
                    };
                    return true;
                }

                l_hit = null;
                return false;
            }

            hit = null;
            return false;
        }

        internal bool RaycastToColliderRough(ref FVector3 origin, ref FVector3 direction,
            FixCollider collider) {
            return CollisionDetection.RayBoundingBoxCollision(ref origin, ref direction, ref collider.shape._boundingBox,
                ref collider._invOrientation, ref collider._position);
        }

        internal bool RaycastToColliderTryFast(ref FVector3 origin, ref FVector3 direction,
            FixCollider collider) {
            // 先使用包围盒做粗糙检测，如果包围盒都没碰撞，那就不用继续检测了，肯定没碰撞
            if (!CollisionDetection.RayBoundingBoxCollision(ref origin, ref direction, ref collider.shape._boundingBox,
                    ref collider._invOrientation, ref collider._position)) {
                return false;
            }

            // 如果该形状和包围盒的贴合度高，则可以使用包围盒的结果作为检测标准。比如box的形状和包围盒就完美贴合
            if (collider.shape.fittingDegreeType == FittingDegreeType.Perfect) {
                return true;
            }

            // 高精度判断，且可以获得详细碰撞信息，但比AABB检测慢3倍左右
            return CollisionDetectionTemp.GJKCollide.Raycast(collider.shape, ref collider._orientation,
                ref collider._invOrientation, ref collider._position, ref origin, ref direction, out var fraction,
                out var normal);
        }

        internal bool RaycastToColliderAccurate(ref FVector3 origin, ref FVector3 direction,
            FixCollider collider, out FVector3 normal, out FLOAT fraction) {
            fraction = Fixp.MaxValue;
            normal = FVector3.Zero;

            if (!CollisionDetection.RayBoundingBoxCollision(ref origin, ref direction, ref collider.shape._boundingBox,
                    ref collider._invOrientation, ref collider._position)) {
                return false;
            }

            return CollisionDetectionTemp.GJKCollide.Raycast(collider.shape, ref collider._orientation,
                ref collider._invOrientation, ref collider._position, ref origin, ref direction, out fraction,
                out normal);
        }
    }
}
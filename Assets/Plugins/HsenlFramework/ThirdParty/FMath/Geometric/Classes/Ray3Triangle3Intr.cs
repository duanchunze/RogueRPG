#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace FixedMath {
    /// <summary>
    /// Contains information about intersection of Ray and Triangle3
    /// </summary>
    public struct Ray3Triangle3Intr {
        /// <summary>
        /// Equals to IntersectionTypes.Point if intersection occured otherwise IntersectionTypes.Empty (even when a ray lies in a triangle plane)
        /// </summary>
        public IntersectionTypes IntersectionType;

        /// <summary>
        /// Intersection point (in case of IntersectionTypes.Point)
        /// </summary>
        public FVector3 Point;

        /// <summary>
        /// Ray evaluation parameter of the intersection point (in case of IntersectionTypes.Point)
        /// </summary>
        public FLOAT RayParameter;

        /// <summary>
        /// First barycentric coordinate of the intersection point
        /// </summary>
        public FLOAT TriBary0;

        /// <summary>
        /// Second barycentric coordinate of the intersection point
        /// </summary>
        public FLOAT TriBary1;

        /// <summary>
        /// Third barycentric coordinate of the intersection point
        /// </summary>
        public FLOAT TriBary2;
    }
}
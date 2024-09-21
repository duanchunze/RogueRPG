#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    /// <summary>
    /// Contains information about intersection of Line3 and Triangle3
    /// </summary>
    public struct Line3Triangle3Intr {
        /// <summary>
        /// Equals to IntersectionTypes.Point if intersection occured otherwise IntersectionTypes.Empty (even when a line lies in a triangle plane)
        /// </summary>
        public IntersectionTypes IntersectionType;

        /// <summary>
        /// Intersection point (in case of IntersectionTypes.Point)
        /// </summary>
        public Vector3 Point;

        /// <summary>
        /// Line evaluation parameter of the intersection point (in case of IntersectionTypes.Point)
        /// </summary>
        public FLOAT LineParameter;

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
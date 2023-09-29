namespace FixedMath {
    /// <summary>
    /// Contains information about intersection of Plane3 and Sphere3
    /// </summary>
    public struct Plane3Sphere3Intr {
        /// <summary>
        /// Equals to IntersectionType.Point if a sphere is touching a plane, IntersectionType.Other if a sphere intersects a plane, otherwise IntersectionType.Empty
        /// </summary>
        public IntersectionTypes IntersectionType;

        /// <summary>
        /// Contains intersection circle of a sphere and a plane in case of IntersectionType.Other
        /// </summary>
        public Circle Circle;
    }
}
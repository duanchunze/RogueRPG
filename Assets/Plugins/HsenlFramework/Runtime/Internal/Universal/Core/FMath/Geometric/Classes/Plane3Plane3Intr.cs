namespace Hsenl {
    /// <summary>
    /// Contains information about intersection of Plane3 and Plane3
    /// </summary>
    public struct Plane3Plane3Intr {
        /// <summary>
        /// Equals to IntersectionTypes.Line or IntersectionTypes.Plane (planes are the same) if intersection occured otherwise IntersectionTypes.Empty.
        /// </summary>
        public IntersectionTypes IntersectionType;

        /// <summary>
        /// Intersection line (in case of IntersectionTypes.Line)
        /// </summary>
        public Line Line;
    }
}
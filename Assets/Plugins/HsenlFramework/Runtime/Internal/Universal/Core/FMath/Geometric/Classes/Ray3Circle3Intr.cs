namespace Hsenl {
    /// <summary>
    /// Contains information about intersection of Ray3 and Circle3 (circle considered to be solid)
    /// </summary>
    public struct Ray3Circle3Intr {
        /// <summary>
        /// Equals to IntersectionTypes.Point if intersection occured otherwise IntersectionTypes.Empty
        /// (including the case when a ray lies in the plane of a circle)
        /// </summary>
        public IntersectionTypes IntersectionType;

        /// <summary>
        /// Intersection point
        /// </summary>
        public Vector3 Point;
    }
}
namespace Hsenl {
    /// <summary>
    /// Contains information about intersection of Line2 and Circle2
    /// </summary>
    public struct Line2Circle2Intr {
        /// <summary>
        /// Equals to IntersectionTypes.Point or IntersectionTypes.Segment
        /// if intersection occured otherwise IntersectionTypes.Empty
        /// </summary>
        public IntersectionTypes IntersectionType;

        /// <summary>
        /// First point of intersection (in case of IntersectionTypes.Point or IntersectionTypes.Segment)
        /// </summary>
        public Vector2 Point0;

        /// <summary>
        /// Second point of intersection (in case of IntersectionTypes.Segment)
        /// </summary>
        public Vector2 Point1;
    }
}
namespace Vidmake.src.positioning.interpolators
{
    /// <summary>
    /// Implements a simple linear interpolation between two double values.
    /// </summary>
    public class LinearInterpolator : IInterpolator
    {
        public static LinearInterpolator Instance { get; } = new();

        private LinearInterpolator()
        {

        }
        /// <summary>
        /// Interpolates between two values a and b based on the given ratio.
        /// The ratio is clamped between 0 and 1.
        /// </summary>
        /// <param name="a">The start value (corresponds to ratio = 0).</param>
        /// <param name="b">The end value (corresponds to ratio = 1).</param>
        /// <param name="ratio">The progress ratio between 0 and 1.</param>
        /// <returns>The interpolated value.</returns>
        public double Interpolate(double a, double b, double ratio)
        {
            // Clamp ratio to [0,1] to prevent overshoot
            if (ratio < 0f) ratio = 0f;
            if (ratio > 1f) ratio = 1f;

            // Linear interpolation formula: value = a + (b - a) * ratio
            return a + (b - a) * ratio;
        }
    }
}

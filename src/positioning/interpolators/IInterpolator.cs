namespace AbstractRendering
{
    /// <summary>
    /// Defines a contract for interpolating between two numeric values.
    /// </summary>
    public interface IInterpolator
    {
        /// <summary>
        /// Computes an interpolated value between <paramref name="a"/> and <paramref name="b"/>
        /// based on a <paramref name="ratio"/> in the range [0, 1].
        /// </summary>
        /// <param name="a">The starting value (ratio = 0).</param>
        /// <param name="b">The ending value (ratio = 1).</param>
        /// <param name="ratio">Progress of the interpolation, usually between 0 and 1.</param>
        /// <returns>The interpolated value.</returns>
        double Interpolate(double a, double b, double ratio);
    }
}

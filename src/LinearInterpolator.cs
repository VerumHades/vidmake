namespace AbstractRendering
{
    public class LinearInterpolator : IInterpolator
    {
        public float Interpolate(float a, float b, float ratio)
        {
            // Clamp ratio to [0,1] to avoid extrapolation
            if (ratio < 0f) ratio = 0f;
            if (ratio > 1f) ratio = 1f;

            return a + (b - a) * ratio;
        }
    }
}
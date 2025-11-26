namespace AbstractRendering
{
    public interface IInterpolator
    {
        float Interpolate(float a, float b, float ratio);
    }
}
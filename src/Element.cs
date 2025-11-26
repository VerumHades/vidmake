using RawRendering;

namespace AbstractRendering
{
    public abstract class Element: TransitionalTransform, ISurface
    {
        public IInterpolator AnimationInterpolator {get;} = new LinearInterpolator();

        public abstract void Render(ref DrawableArea area, float animationPercentage);
    }
}
namespace AbstractRendering
{
    interface ISurface
    {
        public abstract void Render(ref DrawableArea area, float animationPercentage);
    }
}
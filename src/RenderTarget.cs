namespace AbstractRendering
{
    public abstract class RenderTarget
    {
        public abstract int Width {get;}
        public abstract int Height {get;}
        public abstract int TargetFPS {get;}
        //public abstract IDrawableArea GetDrawableArea(int x, int y, int width, int height);
        public abstract void AddElementFrames(IReadOnlyList<Element> elements, int frameCount);
    }
}
namespace AbstractRendering
{
    public class Rectangle : Element
    {
        public Rectangle()
        {
            NextWidth = 10;
            NextHeight = 10;
            ApplyNext();
        }
        public Rectangle(int Width, int Height)
        {
            NextWidth = Width;
            NextHeight = Height;
            ApplyNext();
        }
        public override void Render(ref DrawableArea area, float animationPercentage)
        {
            area.Fill(Pixel.Green);
        }
    }
}
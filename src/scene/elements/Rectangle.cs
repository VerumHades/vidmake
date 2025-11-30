namespace AbstractRendering
{
    /// <summary>
    /// A simple rectangle element that can be added to a Scene.
    /// Inherits animation properties from Element / TransitionalTransform.
    /// Supports a customizable background color.
    /// </summary>
    public class Rectangle : Element
    {
        /// <summary>
        /// The background color of the rectangle.
        /// </summary>
        public Pixel BackgroundColor { get; set; } = Pixel.Green;

        /// <summary>
        /// Default constructor initializes a 10x10 rectangle with default green color.
        /// </summary>
        public Rectangle()
        {
            Width.Next = 10;
            Height.Next = 10;
            ApplyNext();
        }

        /// <summary>
        /// Constructor specifying custom width, height, and optional background color.
        /// </summary>
        /// <param name="width">Initial width of the rectangle.</param>
        /// <param name="height">Initial height of the rectangle.</param>
        /// <param name="backgroundColor">Optional background color (defaults to green).</param>
        public Rectangle(int width, int height, Pixel? backgroundColor = null)
        {
            Width.Next = width;
            Height.Next = height;
            ApplyNext();

            if (backgroundColor.HasValue)
                BackgroundColor = backgroundColor.Value;
        }

        /// <summary>
        /// Draws the rectangle into the provided drawable area.
        /// Fills the area with the rectangle's background color.
        /// </summary>
        public override void Render(ref DrawableArea area, float animationPercentage)
        {
            area.Fill(BackgroundColor);
        }
    }
}

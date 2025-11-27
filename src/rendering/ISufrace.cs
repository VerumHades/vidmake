namespace AbstractRendering
{
    /// <summary>
    /// Represents a drawable surface that can render itself into a DrawableArea.
    /// </summary>
    interface ISurface
    {
        /// <summary>
        /// Renders the surface into the provided drawable area.
        /// </summary>
        /// <param name="area">The target area of pixels to draw into.</param>
        /// <param name="animationPercentage">
        /// The progress of the animation (0.0 = start, 1.0 = end) for this frame.
        /// </param>
        public abstract void Render(ref DrawableArea area, float animationPercentage);
    }
}
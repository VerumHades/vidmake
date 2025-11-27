namespace AbstractRendering
{
    /// <summary>
    /// Represents the actual integer-based position and size of an element in a frame.
    /// </summary>
    public struct PhysicalTransform
    {
        /// <summary>
        /// X position (in pixels) of the element in the frame.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Y position (in pixels) of the element in the frame.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Width (in pixels) of the element.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Height (in pixels) of the element.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Constructor initializing all fields.
        /// </summary>
        /// <param name="x">X position in pixels.</param>
        /// <param name="y">Y position in pixels.</param>
        /// <param name="width">Width in pixels.</param>
        /// <param name="height">Height in pixels.</param>
        public PhysicalTransform(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Returns a readable string representation of the transform.
        /// Useful for debugging.
        /// </summary>
        public override string ToString()
        {
            return $"PhysicalTransform {{ X = {X}, Y = {Y}, Width = {Width}, Height = {Height} }}";
        }
    }
}
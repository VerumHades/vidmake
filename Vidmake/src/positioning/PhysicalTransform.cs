using System;

namespace Vidmake.src.positioning
{
    /// <summary>
    /// Represents the actual integer-based position and size of an element in a frame.
    /// Width and Height must always be positive.
    /// </summary>
    public struct PhysicalTransform
    {
        private int width;
        private int height;

        /// <summary>
        /// X position (in pixels) of the element in the frame.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Y position (in pixels) of the element in the frame.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Width (in pixels) of the element. Must be positive.
        /// </summary>
        public int Width
        {
            get => width;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Width must be positive.");
                width = value;
            }
        }

        /// <summary>
        /// Height (in pixels) of the element. Must be positive.
        /// </summary>
        public int Height
        {
            get => height;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Height must be positive.");
                height = value;
            }
        }

        /// <summary>
        /// Constructor initializing all fields. Width and Height must be positive.
        /// </summary>
        public PhysicalTransform(int x, int y, int width, int height)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width), width, "Width must be positive.");
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height), height, "Height must be positive.");

            X = x;
            Y = y;
            this.width = width;
            this.height = height;
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

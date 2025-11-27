namespace AbstractRendering
{
    /// <summary>
    /// Represents a transform with smooth transitions between states.
    /// Holds both the current state (read-only) and the next target state (modifiable),
    /// allowing interpolation between them over time.
    /// </summary>
    public abstract class TransitionalTransform
    {
        // --- Current state (read-only) ---
        /// <summary>
        /// Current X position (read-only).
        /// Updated only when ApplyNext() is called.
        /// </summary>
        public float CurrentX { get; private set; }

        /// <summary>
        /// Current Y position (read-only).
        /// Updated only when ApplyNext() is called.
        /// </summary>
        public float CurrentY { get; private set; }

        /// <summary>
        /// Current width (read-only).
        /// Updated only when ApplyNext() is called.
        /// </summary>
        public float CurrentWidth { get; private set; }

        /// <summary>
        /// Current height (read-only).
        /// Updated only when ApplyNext() is called.
        /// </summary>
        public float CurrentHeight { get; private set; }

        // --- Next state (read/write) ---
        /// <summary>
        /// Target X position for the next frame or transition.
        /// Can be freely modified.
        /// </summary>
        public float NextX { get; set; }

        /// <summary>
        /// Target Y position for the next frame or transition.
        /// Can be freely modified.
        /// </summary>
        public float NextY { get; set; }

        /// <summary>
        /// Target width for the next frame or transition.
        /// Can be freely modified.
        /// </summary>
        public float NextWidth { get; set; }

        /// <summary>
        /// Target height for the next frame or transition.
        /// Can be freely modified.
        /// </summary>
        public float NextHeight { get; set; }

        /// <summary>
        /// Initializes a new transitional transform with optional starting position and size.
        /// Both Current and Next values are initialized to the same starting values.
        /// </summary>
        public TransitionalTransform(float x = 0, float y = 0, float width = 0, float height = 0)
        {
            CurrentX = NextX = x;
            CurrentY = NextY = y;
            CurrentWidth = NextWidth = width;
            CurrentHeight = NextHeight = height;
        }

        /// <summary>
        /// Computes the interpolated transform for a given ratio using a provided interpolator.
        /// </summary>
        /// <param name="interpolator">The interpolation strategy (linear, easing, etc.).</param>
        /// <param name="ratio">Progress ratio between 0 (current) and 1 (next).</param>
        /// <returns>A PhysicalTransform representing the interpolated state.</returns>
        public PhysicalTransform GetInterpolated(IInterpolator interpolator, float ratio)
        {
            return new()
            {
                X = (int)interpolator.Interpolate(CurrentX, NextX, ratio),
                Y = (int)interpolator.Interpolate(CurrentY, NextY, ratio),
                Width = (int)interpolator.Interpolate(CurrentWidth, NextWidth, ratio),
                Height = (int)interpolator.Interpolate(CurrentHeight, NextHeight, ratio)
            };
        }

        /// <summary>
        /// Copies all "Next" values into the "Current" values.
        /// Effectively commits the next state as the current state.
        /// </summary>
        public void ApplyNext()
        {
            CurrentX = NextX;
            CurrentY = NextY;
            CurrentWidth = NextWidth;
            CurrentHeight = NextHeight;
        }

        /// <summary>
        /// Sets the next position (NextX, NextY) to a new location.
        /// </summary>
        /// <param name="x">Target X position.</param>
        /// <param name="y">Target Y position.</param>
        public void Move(float x, float y)
        {
            NextX = x;
            NextY = y;
        }

        /// <summary>
        /// Sets the next size (NextWidth, NextHeight) to a new width and height.
        /// </summary>
        /// <param name="width">Target width.</param>
        /// <param name="height">Target height.</param>
        public void Resize(float width, float height)
        {
            NextWidth = width;
            NextHeight = height;
        }
    }
}

namespace AbstractRendering
{
    public abstract class TransitionalTransform
    {
        // --- Current (read-only) ---
        public float CurrentX { get; private set; }
        public float CurrentY { get; private set; }
        public float CurrentWidth { get; private set; }
        public float CurrentHeight { get; private set; }

        // --- Next (read/write) ---
        public float NextX { get; set; }
        public float NextY { get; set; }
        public float NextWidth { get; set; }
        public float NextHeight { get; set; }

        public TransitionalTransform(float x = 0, float y = 0, float width = 0, float height = 0)
        {
            CurrentX = NextX = x;
            CurrentY = NextY = y;
            CurrentWidth = NextWidth = width;
            CurrentHeight = NextHeight = height;
        }

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
        /// Copies all "Next" values into "Current".
        /// </summary>
        public void ApplyNext()
        {
            CurrentX = NextX;
            CurrentY = NextY;
            CurrentWidth = NextWidth;
            CurrentHeight = NextHeight;
        }

        /// <summary>
        /// Sets the next position (NextX, NextY).
        /// </summary>
        public void Move(float x, float y)
        {
            NextX = x;
            NextY = y;
        }

        /// <summary>
        /// Sets the next size (NextWidth, NextHeight).
        /// </summary>
        public void Resize(float width, float height)
        {
            NextWidth = width;
            NextHeight = height;
        }
    }
}
namespace AbstractRendering
{
    /// <summary>
    /// Represents a transform whose state transitions smoothly between frames.
    /// Uses TransitionalProperty to store independent current/next values for
    /// position and size, enabling easy interpolation.
    /// </summary>
    public abstract class TransitionalTransform
    {
        // Position properties
        public TransitionalProperty<float> X { get; }
        public TransitionalProperty<float> Y { get; }

        // Size properties
        public TransitionalProperty<float> Width { get; }
        public TransitionalProperty<float> Height { get; }

        /// <summary>
        /// Creates a new TransitionalTransform with starting values.
        /// Current and Next values for all properties are initialized to the same values.
        /// </summary>
        public TransitionalTransform(float x = 0, float y = 0, float width = 0, float height = 0)
        {
            X = new TransitionalProperty<float>(x);
            Y = new TransitionalProperty<float>(y);
            Width = new TransitionalProperty<float>(width);
            Height = new TransitionalProperty<float>(height);
        }

        /// <summary>
        /// Computes an interpolated transform using the provided interpolation method.
        /// </summary>
        public PhysicalTransform GetInterpolated(IInterpolator interpolator, float ratio)
        {
            return new()
            {
                X = (int)interpolator.Interpolate(X.Current, X.Next, ratio),
                Y = (int)interpolator.Interpolate(Y.Current, Y.Next, ratio),
                Width = (int)interpolator.Interpolate(Width.Current, Width.Next, ratio),
                Height = (int)interpolator.Interpolate(Height.Current, Height.Next, ratio)
            };
        }

        /// <summary>
        /// Commits all next-state values to current-state values.
        /// </summary>
        public virtual void ApplyNext()
        {
            X.ApplyNext();
            Y.ApplyNext();
            Width.ApplyNext();
            Height.ApplyNext();
        }

        /// <summary>
        /// Sets the next position.
        /// </summary>
        public void Move(float x, float y)
        {
            X.Next = x;
            Y.Next = y;
        }

        /// <summary>
        /// Sets the next size.
        /// </summary>
        public void Resize(float width, float height)
        {
            Width.Next = width;
            Height.Next = height;
        }
    }
}
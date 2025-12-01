namespace AbstractRendering
{
    /// <summary>
    /// Abstract base class representing a rendering target for a scene.
    /// A RenderTarget is responsible for producing frames for elements over time.
    /// </summary>
    public abstract class RenderTarget
    {
        public IRenderStateReporter? RenderStateReporter {get;set;}
        /// <summary>
        /// Generates frames for a collection of elements over a specified duration.
        /// Each element may have "Current" and "Next" states, which should be
        /// interpolated or applied over the given time span.
        /// </summary>
        /// <param name="createElementEnumerator">Creates an enumerator that should be saved to use in multiple threads.</param>
        /// <param name="seconds">The duration (in seconds) to advance the scene.</param>
        public abstract void AddElementFrames(Func<IEnumerable<Element>> createElementEnumerator, int seconds);
    }
}
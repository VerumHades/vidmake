namespace Vidmake.src.positioning
{
    /// <summary>
    /// Represents a property that holds a current value and a queued "next" value.
    /// </summary>
    /// <typeparam name="T">The type of the stored values.</typeparam>
    public class TransitionalProperty<T>(T current)
    {
        /// <summary>
        /// Gets or sets the next value to be applied.
        /// </summary>
        public T Next { get; set; } = current;

        /// <summary>
        /// Gets the current active value.
        /// </summary>
        public T Current { get; private set; } = current;

        /// <summary>
        /// Applies the <see cref="Next"/> value to <see cref="Current"/>.
        /// </summary>
        public void ApplyNext()
        {
            Current = Next;
        }
    }
}
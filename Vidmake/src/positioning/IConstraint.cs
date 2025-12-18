namespace Vidmake.src.positioning
{
    /// <summary>
    /// Represents a constraint for a value of type T.
    /// </summary>
    /// <typeparam name="T">The type of value to validate.</typeparam>
    public interface IConstraint<T>
    {
        /// <summary>
        /// Validates the value.
        /// Should throw an exception if the value is invalid.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        void Validate(T value);
    }
}

using System;
using Vidmake.src.positioning;

namespace Vidmake.src.positioning.constraints
{
    /// <summary>
    /// Ensures that a value of a comparable type is strictly greater than zero.
    /// Works for int, long, double, decimal, etc.
    /// </summary>
    /// <typeparam name="T">A comparable type.</typeparam>
    public class PositiveComparableConstraint<T> : IConstraint<T> where T : IComparable<T>
    {
        private readonly T zero;

        /// <summary>
        /// Initializes the constraint with an optional "zero" value.
        /// Default is default(T).
        /// </summary>
        /// <param name="zeroValue">The value considered as zero for comparison.</param>
        public PositiveComparableConstraint(T zeroValue = default!)
        {
            zero = zeroValue;
        }

        public void Validate(T value)
        {
            if (value.CompareTo(zero) <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), value, $"Value {value} must be greater than {zero}.");
        }
    }
}
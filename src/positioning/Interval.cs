namespace Vidmake.src.positioning
{
    /// <summary>
    /// Represents a strictly bounded interval (Start, End) using a comparable type.
    /// </summary>
    /// <typeparam name="T">A type that implements <see cref="IComparable{T}"/>.</typeparam>
    public class Interval<T> where T : IComparable<T>
    {
        private T start;
        private T end;

        /// <summary>
        /// Gets or sets the start of the interval.
        /// Must always be strictly less than <see cref="End"/>.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the value is greater than or equal to <see cref="End"/>.</exception>
        public T Start
        {
            get => start;
            set
            {
                if (value.CompareTo(end) >= 0)
                    throw new ArgumentException("Start must be less than End.");
                start = value;
            }
        }

        /// <summary>
        /// Gets or sets the end of the interval.
        /// Must always be strictly greater than <see cref="Start"/>.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the value is less than or equal to <see cref="Start"/>.</exception>
        public T End
        {
            get => end;
            set
            {
                if (value.CompareTo(start) <= 0)
                    throw new ArgumentException("End must be greater than Start.");
                end = value;
            }
        }

        /// <summary>
        /// Creates a new interval (start, end).
        /// </summary>
        /// <param name="start">The lower bound of the interval.</param>
        /// <param name="end">The upper bound of the interval.</param>
        /// <exception cref="ArgumentException">Thrown when start is greater than or equal to end.</exception>
        public Interval(T start, T end)
        {
            if (start.CompareTo(end) >= 0)
                throw new ArgumentException("Start must be less than End.");
            this.start = start;
            this.end = end;
        }

        /// <summary>
        /// Determines whether a value lies strictly inside this interval.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <returns><c>true</c> if start &lt; value &lt; end; otherwise, <c>false</c>.</returns>
        public bool Contains(T value) => value.CompareTo(start) > 0 && value.CompareTo(end) < 0;

        /// <summary>
        /// Returns a string representation of the interval.
        /// </summary>
        public override string ToString() => $"({start}, {end})";
    }
}
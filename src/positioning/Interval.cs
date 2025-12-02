namespace Vidmake.src.positioning
{
    public class Interval<T> where T : IComparable<T>
    {
        private T start;
        private T end;

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

        public Interval(T start, T end)
        {
            if (start.CompareTo(end) >= 0)
                throw new ArgumentException("Start must be less than End.");
            this.start = start;
            this.end = end;
        }

        public bool Contains(T value) => value.CompareTo(start) > 0 && value.CompareTo(end) < 0;

        public override string ToString() => $"({start}, {end})";
    }
}
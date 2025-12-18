using System;

namespace Vidmake.src.positioning.constraints
{
    public class RangeConstraint : IConstraint<int>
    {
        public int Min { get; }
        public int Max { get; }

        public RangeConstraint(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public void Validate(int value)
        {
            if (value < Min)
                throw new ArgumentOutOfRangeException(nameof(value), value, $"Value {value} is less than minimum {Min}.");
            if (value > Max)
                throw new ArgumentOutOfRangeException(nameof(value), value, $"Value {value} is greater than maximum {Max}.");
        }
    }
}

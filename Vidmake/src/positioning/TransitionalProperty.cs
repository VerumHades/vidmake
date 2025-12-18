using System;

namespace Vidmake.src.positioning
{
    public class TransitionalProperty<T>
    {
        private T current;
        private T next;
        private readonly IConstraint<T>? constraint;

        public TransitionalProperty(T initialValue, IConstraint<T>? constraint = null)
        {
            this.constraint = constraint;
            constraint?.Validate(initialValue);

            current = initialValue;
            next = initialValue;
        }

        public T Next
        {
            get => next;
            set
            {
                constraint?.Validate(value);
                next = value;
            }
        }

        public T Current => current;

        public void ApplyNext()
        {
            constraint?.Validate(next);
            current = next;
        }
    }
}

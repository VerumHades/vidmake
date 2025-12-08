namespace Vidmake.src
{
    /// <summary>
    /// A generic observable property that notifies listeners whenever its value changes.
    /// </summary>
    public class ObservableProperty<T>(T value)
    {
        /// <summary>
        /// A list of actions to invoke when the value changes.
        /// Each listener receives the new value as a parameter.
        /// </summary>
        public List<Action<T>> Changed { get; } = new();

        /// <summary>
        /// Registers a listener to be invoked whenever the Value changes.
        /// </summary>
        /// <param name="listener">Callback invoked with the updated value.</param>
        public void AddChangedListener(Action<T> listener)
        {
            Changed.Add(listener);
        }

        // Backing field for the observable value.
        private T value = value;

        /// <summary>
        /// The observable value. Setting this triggers all registered listeners.
        /// </summary>
        public T Value
        {
            get
            {
                return value;
            }
            set
            {
                foreach (var listener in Changed)
                    listener?.Invoke(value);

                this.value = value;
            }
        }
    }
}
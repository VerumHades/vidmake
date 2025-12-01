namespace AbstractRendering
{
    public class ObservableProperty<T>(T value)
    {
        public List<Action<T>> Changed {get;} = new();
    
        public void AddChangedListener(Action<T> listener)
        {
            Changed.Add(listener);
        }

        private T value = value;
        public T Value
        {
            get
            {
                return value;
            }
            set
            {
                foreach(var listener in Changed)
                    listener?.Invoke(value);

                this.value = value;
            }
        }
    }
}
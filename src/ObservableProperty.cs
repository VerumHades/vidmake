namespace AbstractRendering
{
    public class ObservableProperty<T>(T value)
    {
        public Action<T>? Changed {get;set;}

        private T value = value;
        public T Value
        {
            get
            {
                return value;
            }
            set
            {
                Changed?.Invoke(value);
                this.value = value;
            }
        }
    }
}
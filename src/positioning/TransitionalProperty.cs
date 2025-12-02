namespace Vidmake.src.positioning
{
    public class TransitionalProperty<T>(T current)
    {
        public T Next { get; set; } = current;
        public T Current { get; private set; } = current;

        public void ApplyNext()
        {
            Current = Next;
        }
    }
}
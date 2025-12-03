namespace Vidmake.src.logging
{
    public interface IReporter
    {
        public void Message(string message);
        public void Error(string message);
    }
}
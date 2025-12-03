namespace Vidmake.src.logging
{
    public interface IStandardOutputReporter
    {
        public void Message(string message);
        public void Error(string message);
    }
}
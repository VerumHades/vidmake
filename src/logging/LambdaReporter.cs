namespace Vidmake.src.logging
{
    public class LambdaReporter : IReporter
    {
        public Action<string>? ErrorHandler { get; set; }
        public Action<string>? MessageHandler { get; set; }

        public void Error(string message)
        {
            ErrorHandler?.Invoke(message);
        }

        public void Message(string message)
        {
            MessageHandler?.Invoke(message);
        }
    }
}
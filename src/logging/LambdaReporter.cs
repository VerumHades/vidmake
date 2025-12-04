namespace Vidmake.src.logging
{
    public class LambdaReporter : IReporter
    {
        public Action<string>? ErrorHandler { get; set; }
        public Action<string>? MessageHandler { get; set; }
        public Action<string>? WarningHandler { get; set; }

        public void Error(string message)
        {
            ErrorHandler?.Invoke(message);
        }

        public void Message(string message)
        {
            MessageHandler?.Invoke(message);
        }

        public void Warn(string message)
        {
            WarningHandler?.Invoke(message);
        }
    }
}
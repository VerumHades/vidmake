namespace Vidmake.src.logging {
    public class NullReporter : IReporter
    {
        public static NullReporter Instance {get; } = new NullReporter();
        private NullReporter()
        {
            
        }
        public void Error(string message)
        {
            
        }

        public void Message(string message)
        {
            
        }

        public void Warn(string message)
        {
            
        }
    }
}
namespace Vidmake.src.logging
{
    public abstract class LoggingProbe
    {
        public IReporter Reporter {get;set;}

        public LoggingProbe(IReporter reporter)
        {
            Reporter = reporter;
        }

        protected void Message(string message)
        {
            Reporter.Message(message);
        }
        protected void Error(string error)
        {
            Reporter.Error(error);
        }
    }
}
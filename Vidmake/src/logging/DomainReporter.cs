namespace Vidmake.src.logging
{
    public class DomainReporter
    {
        private readonly IReporter mainReporter;

        // name -> (reporter, disabled)
        private readonly Dictionary<string, (LambdaReporter reporter, bool disabled)> reporters = new();

        public DomainReporter(IReporter mainReporter)
        {
            this.mainReporter = mainReporter;
        }

        /// <summary>
        /// Creates a new reporter bound to a name. Starts enabled.
        /// </summary>
        public IReporter NewReporter(string name)
        {
            var reporter = new LambdaReporter();

            // When enabled, funnel into main reporter
            reporter.MessageHandler = msg =>
            {
                if (!reporters[name].disabled)
                    mainReporter.Message($"[{name}] {msg}");
            };

            reporter.WarningHandler = msg =>
            {
                if (!reporters[name].disabled)
                    mainReporter.Warn($"[{name}] {msg}");
            };

            reporter.ErrorHandler = msg =>
            {
                if (!reporters[name].disabled)
                    mainReporter.Error($"[{name}] {msg}");
            };

            reporters[name] = (reporter, false); // enabled by default
            return reporter;
        }

        public T Add<T>(string name, T reportable) where T: IReportable
        {
            reportable.Reporter = NewReporter(name);
            return reportable;
        }

        public void Enable(string name)
        {
            if (reporters.TryGetValue(name, out var entry))
                reporters[name] = (entry.reporter, false);
        }

        public void Disable(string name)
        {
            if (reporters.TryGetValue(name, out var entry))
                reporters[name] = (entry.reporter, true);
        }

        public bool IsEnabled(string name)
        {
            if (reporters.TryGetValue(name, out var entry))
                return !entry.disabled;

            return false;
        }
    }
}
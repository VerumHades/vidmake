namespace Vidmake.src.logging
{
    public class ConsoleReporter : IReporter
    {
        public bool UseColors { get; set; }

        public ConsoleReporter(bool useColors = true)
        {
            UseColors = useColors;
        }

        public void Error(string message)
        {
            if (UseColors)
            {
                var old = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(message);
                Console.ForegroundColor = old;
            }
            else
            {
                Console.WriteLine(message);
            }
        }

        public void Message(string message)
        {
            if (UseColors)
            {
                var old = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(message);
                Console.ForegroundColor = old;
            }
            else
            {
                Console.WriteLine(message);
            }
        }
    }
}
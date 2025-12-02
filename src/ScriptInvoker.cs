using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Vidmake.src.scene.elements;

namespace Vidmake.src
{
    /// <summary>
    /// Handles execution of external C# scripts with access to a provided API.
    /// </summary>
    public class ScriptInvoker<TApi> where TApi : class
    {
        /// <summary>
        /// The API instance that scripts can use.
        /// </summary>
        public TApi Api { get; }

        /// <summary>
        /// Creates a new ScriptInvoker for a given API object.
        /// </summary>
        /// <param name="api">The API object scripts will interact with.</param>
        public ScriptInvoker(TApi api)
        {
            Api = api ?? throw new ArgumentNullException(nameof(api));
        }

        /// <summary>
        /// Executes a C# script synchronously.
        /// </summary>
        /// <param name="scriptCode">The C# script code to execute.</param>
        public void Execute(string scriptCode)
        {
            if (string.IsNullOrWhiteSpace(scriptCode))
                throw new ArgumentException("Script code cannot be empty.", nameof(scriptCode));

            var options = ScriptOptions.Default
                .WithImports("System", "System.Math", "Vidmake.src", "Vidmake.src.scene", "Vidmake.src.scene.elements", "Vidmake.src.positioning")
                .WithReferences(typeof(TApi).Assembly)
                .WithReferences(typeof(Rectangle).Assembly)
                .WithReferences(typeof(Pixel).Assembly)
                .WithReferences(typeof(Plot2D).Assembly)
                .WithReferences(typeof(TApi).Assembly);

            try
            {
                // Synchronous execution
                CSharpScript.RunAsync(scriptCode, options: options, globals: Api)
                           .GetAwaiter().GetResult();
            }
            catch (CompilationErrorException e)
            {
                // Red text for compilation errors
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(new string('=', 60));
                Console.WriteLine(" SCRIPT COMPILATION ERROR ");
                Console.WriteLine(new string('=', 60));

                foreach (var diag in e.Diagnostics)
                    Console.WriteLine($"  - {diag}");

                Console.ResetColor();
                throw;
            }
            catch (Exception ex)
            {
                // Yellow text for runtime errors
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(new string('=', 60));
                Console.WriteLine(" SCRIPT EXECUTION ERROR ");
                Console.WriteLine(new string('=', 60));

                Console.WriteLine($"  {ex.Message}");
                Console.ResetColor();
                throw;
            }
        }
    }
}
using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace AbstractRendering
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
                .WithImports("System", "System.Math", "AbstractRendering")
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
                Console.WriteLine("Script compilation error:");
                foreach (var diag in e.Diagnostics)
                    Console.WriteLine(diag.ToString());
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Script execution error: {ex.Message}");
                throw;
            }
        }
    }
}
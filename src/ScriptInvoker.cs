using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Vidmake.src.logging;

namespace Vidmake.src
{
    /// <summary>
    /// Handles execution of external C# scripts with access to a provided API.
    /// </summary>
    public class ScriptInvoker<TApi>: IReportable where TApi : class
    {
        public TApi Api { get; }
        public IReporter Reporter { get; set; } = NullReporter.Instance;

        private readonly IEnumerable<string> imports;
        private readonly IEnumerable<Assembly> references;

        /// <summary>
        /// Creates a new ScriptInvoker for a given API object.
        /// Allows specifying custom imports and assembly references.
        /// </summary>
        public ScriptInvoker(
            TApi api,
            IEnumerable<string>? imports = null,
            IEnumerable<Assembly>? references = null
            )
        {
            Api = api ?? throw new ArgumentNullException(nameof(api));



            this.imports = imports ?? new[]
            {
                "System",
                "System.Math",
            };
            
            this.references = references ?? new[]
            {
                typeof(TApi).Assembly,
            };
        }

        /// <summary>
        /// Executes a C# script synchronously.
        /// </summary>
        public void Execute(string scriptCode)
        {
            if (string.IsNullOrWhiteSpace(scriptCode))
                throw new ArgumentException("Script code cannot be empty.", nameof(scriptCode));

            var options = ScriptOptions.Default
                .WithImports(imports)
                .WithReferences(references);

            try
            {
                CSharpScript.RunAsync(scriptCode, options: options, globals: Api)
                           .GetAwaiter().GetResult();
            }
            catch (CompilationErrorException e)
            {
                Reporter.Error(new string('=', 60));
                Reporter.Error(" SCRIPT COMPILATION ERROR ");
                Reporter.Error(new string('=', 60));

                foreach (var diag in e.Diagnostics)
                    Reporter.Error($"  - {diag}");
                
                throw;
            }
            catch (Exception ex)
            {
                Reporter.Error(new string('=', 60));
                Reporter.Error(" SCRIPT EXECUTION ERROR ");
                Reporter.Error(new string('=', 60));

                Reporter.Error($"  {ex.Message}");


                throw;
            }
        }
    }
}

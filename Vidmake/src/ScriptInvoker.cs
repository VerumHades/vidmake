using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Vidmake.src.logging;

namespace Vidmake.src
{
    /// <summary>
    /// Handles execution of external C# scripts with access to a provided API.
    /// </summary>
    public class ScriptInvoker<TApi> where TApi : class
    {
        public TApi Api { get; }

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
                var builder = new StringBuilder();
                
                builder.AppendLine(new string('=', 60));
                builder.AppendLine(" SCRIPT COMPILATION ERROR ");
                builder.AppendLine(new string('=', 60));

                foreach (var diag in e.Diagnostics)
                    builder.AppendLine($"  - {diag}");
                
                throw new Exception(builder.ToString());
            }
            catch (Exception ex)
            {
                var builder = new StringBuilder();

                builder.AppendLine(new string('=', 60));
                builder.AppendLine(" SCRIPT EXECUTION ERROR ");
                builder.AppendLine(new string('=', 60));

                builder.AppendLine($"  {ex.Message}");

                throw new Exception(builder.ToString());
            }
        }
    }
}

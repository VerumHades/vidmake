using System.Reflection;
using System.Text.Json;

namespace Vidmake.src.cli
{
    public static class ConfigLoader<T> where T : new()
    {
        /// <summary>
        /// Loads config from JSON (if --config is present) and applies CLI overrides.
        /// </summary>
        public static T Load(string[] args)
        {
            var config = new T();

            string? configPath = ReadConfigPath(args);
            if (configPath != null)
                config = LoadFromJson(config, configPath);

            ApplyCliOverrides(config, args);

            return config;
        }

        private static string? ReadConfigPath(string[] args)
        {
            for (int i = 0; i < args.Length - 1; i++)
                if (args[i] == "--config")
                    return args[i + 1];

            return null;
        }

        private static T LoadFromJson(T existingConfig, string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Config file not found: {path}");

            var text = File.ReadAllText(path);
            return JsonSerializer.Deserialize<T>(text) ?? existingConfig;
        }

        private static void ApplyCliOverrides(T config, string[] args)
        {
            var dict = ParseCliArgs(args);

            foreach (var prop in typeof(T).GetProperties())
            {
                var attr = prop.GetCustomAttribute<CliOptionAttribute>();
                if (attr == null) continue;

                if (!dict.TryGetValue(attr.Name, out var value))
                    continue;

                object converted;

                if (prop.PropertyType == typeof(bool))
                {
                    converted = string.IsNullOrEmpty(value) ? true : bool.Parse(value);
                }
                else
                {
                    converted = Convert.ChangeType(value, prop.PropertyType);
                }

                prop.SetValue(config, converted);
            }
        }

        private static Dictionary<string, string?> ParseCliArgs(string[] args)
        {
            var dict = new Dictionary<string, string?>();

            for (int i = 0; i < args.Length; i++)
            {
                var key = args[i];

                if (!key.StartsWith('-'))
                    continue;

                if (i + 1 < args.Length && !args[i + 1].StartsWith('-'))
                    dict[key] = args[i + 1];
                else
                    dict[key] = null; // boolean flag
            }

            return dict;
        }
    }
}
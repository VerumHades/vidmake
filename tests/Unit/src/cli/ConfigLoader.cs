using System;
using System.IO;
using System.Text.Json;
using Xunit;
using Vidmake.src.cli;
using System.Reflection;
using System.Text.Json.Serialization;
namespace Vidmake.src.tests
{

    /// <summary>
    /// A synthetic configuration definition designed specifically for testing
    /// the configuration loader. This class includes all argument types,
    /// validation rules, and default behaviors to ensure comprehensive coverage.
    /// </summary>
    public class TestConfig
    {
        [JsonPropertyName("count")]
        [CliOption("--count", "-c", "A positive integer value", ValidationType.MustBePositive)]
        public int PositiveCount { get; set; }

        [JsonPropertyName("sizeBytes")]
        [CliOption("--size-bytes", "-sb", "A positive long value", ValidationType.MustBePositive)]
        public long PositiveSizeBytes { get; set; }

        [JsonPropertyName("name")]
        [CliOption("--name", "-n", "A required non-empty string", ValidationType.NonEmptyString)]
        public string RequiredName { get; set; }

        [JsonPropertyName("optionalTag")]
        [CliOption("--tag", "-t", "An optional string with no validation")]
        public string OptionalTag { get; set; }

        [JsonPropertyName("enableFeature")]
        [CliOption("--enable-feature", "-ef", "Boolean toggle for enabling a feature")]
        public bool EnableFeature { get; set; }

        [JsonPropertyName("disableMode")]
        [CliOption("--disable-mode", "-dm", "Boolean toggle for disabling a mode")]
        public bool DisableMode { get; set; }

        [JsonPropertyName("defaultLevel")]
        [CliOption("--level", "-l", "A defaulted integer level", ValidationType.MustBePositive)]
        public int DefaultLevel { get; set; } = 5;

        [JsonPropertyName("path")]
        [CliOption("--path", "-p", "A required file path", ValidationType.NonEmptyString)]
        public string RequiredPath { get; set; }

        [JsonPropertyName("ratio")]
        [CliOption("--ratio", "-r", "A positive integer ratio", ValidationType.MustBePositive)]
        public int Ratio { get; set; }
    }

    public class ConfigLoaderTests
    {
        private readonly string temporaryDirectoryPath;

        public ConfigLoaderTests()
        {
            temporaryDirectoryPath = Path.Combine(Path.GetTempPath(), "ConfigLoaderTest");
            Directory.CreateDirectory(temporaryDirectoryPath);
        }

        /// <summary>
        /// Ensures that basic JSON file loading populates properties correctly.
        /// </summary>
        [Fact]
        public void Load_FromJsonFile_ShouldPopulateProperties()
        {
            string jsonText = """
            {
                "count": 10,
                "sizeBytes": 1000,
                "name": "JsonName",
                "optionalTag": "JsonTag",
                "enableFeature": true,
                "disableMode": false,
                "defaultLevel": 50,
                "path": "/abc",
                "ratio": 3
            }
            """;

            string filePath = Path.Combine(temporaryDirectoryPath, "config.json");
            File.WriteAllText(filePath, jsonText);

            ConfigLoader<TestConfig> configLoader = new ConfigLoader<TestConfig>();
            TestConfig loadedConfig = configLoader.Load(new[] { "--config", filePath });

            Assert.Equal(10, loadedConfig.PositiveCount);
            Assert.Equal(1000, loadedConfig.PositiveSizeBytes);
            Assert.Equal("JsonName", loadedConfig.RequiredName);
            Assert.Equal("JsonTag", loadedConfig.OptionalTag);
            Assert.True(loadedConfig.EnableFeature);
            Assert.False(loadedConfig.DisableMode);
            Assert.Equal(50, loadedConfig.DefaultLevel);
            Assert.Equal("/abc", loadedConfig.RequiredPath);
            Assert.Equal(3, loadedConfig.Ratio);
        }

        /// <summary>
        /// Ensures that a missing JSON file triggers FileNotFoundException.
        /// </summary>
        [Fact]
        public void Load_FromMissingConfigFile_ShouldThrow()
        {
            string missingPath = Path.Combine(temporaryDirectoryPath, "does_not_exist.json");

            ConfigLoader<TestConfig> configLoader = new ConfigLoader<TestConfig>();

            Assert.Throws<FileNotFoundException>(() =>
            {
                configLoader.Load(new[] { "--config", missingPath });
            });
        }

        /// <summary>
        /// Ensures that CLI overrides replace JSON-loaded values.
        /// </summary>
        [Fact]
        public void Load_JsonAndCli_ShouldApplyCliOverrides()
        {
            string jsonText = """
            {
                "count": 10,
                "sizeBytes": 1000,
                "name": "FromJson",
                "path": "/json",
                "ratio": 2
            }
            """;

            string filePath = Path.Combine(temporaryDirectoryPath, "config2.json");
            File.WriteAllText(filePath, jsonText);

            string[] cliArgs =
            {
                "--config", filePath,
                "--count", "123",
                "--name", "FromCli",
                "--path", "/cli"
            };

            ConfigLoader<TestConfig> configLoader = new ConfigLoader<TestConfig>();
            TestConfig loadedConfig = configLoader.Load(cliArgs);

            Assert.Equal(123, loadedConfig.PositiveCount);
            Assert.Equal(1000, loadedConfig.PositiveSizeBytes);
            Assert.Equal("FromCli", loadedConfig.RequiredName);
            Assert.Equal("/cli", loadedConfig.RequiredPath);
            Assert.Equal(2, loadedConfig.Ratio);
        }

        /// <summary>
        /// Ensures that CLI-only loads populate properties without JSON.
        /// </summary>
        [Fact]
        public void Load_CliOnly_ShouldPopulateProperties()
        {
            string[] cliArgs =
            {
            "--count", "8",
            "--size-bytes", "555",
            "--name", "OnlyCli",
            "--path", "/cli",
            "--ratio", "9",
            "--enable-feature"
        };

            ConfigLoader<TestConfig> configLoader = new ConfigLoader<TestConfig>();
            TestConfig loadedConfig = configLoader.Load(cliArgs);

            Assert.Equal(8, loadedConfig.PositiveCount);
            Assert.Equal(555, loadedConfig.PositiveSizeBytes);
            Assert.Equal("OnlyCli", loadedConfig.RequiredName);
            Assert.Equal("/cli", loadedConfig.RequiredPath);
            Assert.Equal(9, loadedConfig.Ratio);
            Assert.True(loadedConfig.EnableFeature);
        }

        /// <summary>
        /// Ensures that boolean flags without values default to true.
        /// </summary>
        [Fact]
        public void Load_CliBooleanFlagWithoutValue_ShouldBeTrue()
        {
            string[] cliArgs =
            {
                "--name", "Test",
                "--path", "/tmp",
                "--count", "1",
                "--size-bytes", "1",
                "--ratio", "1",
                "--enable-feature"
            };

            ConfigLoader<TestConfig> configLoader = new ConfigLoader<TestConfig>();
            TestConfig loadedConfig = configLoader.Load(cliArgs);

            Assert.True(loadedConfig.EnableFeature);
        }

        /// <summary>
        /// Ensures that unknown CLI options throw ArgumentException.
        /// </summary>
        [Fact]
        public void Load_UnknownOption_ShouldThrow()
        {
            string[] cliArgs =
            {
                "--unknown","value"
            };

            ConfigLoader<TestConfig> configLoader = new ConfigLoader<TestConfig>();

            Assert.Throws<ArgumentException>(() =>
            {
                configLoader.Load(cliArgs);
            });
        }

        /// <summary>
        /// Ensures that validated fields trigger validation failures.
        /// </summary>
        [Fact]
        public void Load_InvalidValues_ShouldTriggerValidation()
        {
            string[] cliArgs =
            {
                "--count", "-5",
                "--size-bytes", "1",
                "--name", "A",
                "--path", "/p",
                "--ratio", "1"
            };

            ConfigLoader<TestConfig> configLoader = new ConfigLoader<TestConfig>();

            Assert.Throws<ArgumentException>(() =>
            {
                configLoader.Load(cliArgs);
            });
        }

        /// <summary>
        /// Ensures that short names also work for CLI overrides.
        /// </summary>
        [Fact]
        public void Load_ShortOptions_ShouldApplyOverrides()
        {
            string[] cliArgs =
            {
                "-c", "20",
                "-sb", "888",
                "-n", "Short",
                "-p", "/short",
                "-r", "4"
            };

            ConfigLoader<TestConfig> configLoader = new ConfigLoader<TestConfig>();
            TestConfig loadedConfig = configLoader.Load(cliArgs);

            Assert.Equal(20, loadedConfig.PositiveCount);
            Assert.Equal(888, loadedConfig.PositiveSizeBytes);
            Assert.Equal("Short", loadedConfig.RequiredName);
            Assert.Equal("/short", loadedConfig.RequiredPath);
            Assert.Equal(4, loadedConfig.Ratio);
        }

        /// <summary>
        /// Ensures that default property values are preserved when not overridden.
        /// </summary>
        [Fact]
        public void Load_Defaults_ShouldBePreserved()
        {
            string[] cliArgs ={
                "--name", "Default",
                "--path", "/def",
                "--count", "3",
                "--size-bytes", "9",
                "--ratio", "1"
            };

            ConfigLoader<TestConfig> configLoader = new ConfigLoader<TestConfig>();
            TestConfig loadedConfig = configLoader.Load(cliArgs);

            Assert.Equal(5, loadedConfig.DefaultLevel);
        }

        /// <summary>
        /// Ensures that a non-boolean CLI option missing its required value
        /// results in a validation failure and throws an exception.
        /// </summary>
        [Fact]
        public void Load_NonBooleanOptionMissingValue_ShouldThrow()
        {
            string[] commandLineArguments =
            {
                "--count" // missing the value that must follow
            };

            ConfigLoader<TestConfig> configLoader = new ConfigLoader<TestConfig>();

            Assert.Throws<ArgumentException>(() =>
            {
                configLoader.Load(commandLineArguments);
            });
        }


        /// <summary>
        /// Ensures that arguments which do not begin with '-' and are not valid
        /// values for a preceding option cause the loader to throw an exception.
        /// </summary>
        [Fact]
        public void Load_ArgumentNotStartingWithDash_ShouldThrow()
        {
            string[] commandLineArguments =
            {
                "--count", "10",
                "unexpected", // invalid: not a flag and not a value position
                "asdawdaw"
            };

            ConfigLoader<TestConfig> configLoader = new ConfigLoader<TestConfig>();

            Assert.Throws<ArgumentException>(() =>
            {
                configLoader.Load(commandLineArguments);
            });
        }
    }
}

using System.Drawing;
using System.Reflection;
using System.Text.Json;
using Vidmake.src;
using Vidmake.src.cli;
using Vidmake.src.logging;
using Vidmake.src.rendering;
using Vidmake.src.rendering.writers;
using Vidmake.src.scene;
using Vidmake.src.scene.elements;

static class Program
{
    private const int ExitSuccess = 0;
    private const int ExitConfigError = 1;
    private const int ExitMissingParameters = 2;
    private const int ExitFileNotFound = 3;
    private const int ExitInvalidOperation = 4;
    private const int ExitOverflow = 5;
    private const int ExitUnexpected = 6;

    static int Main(string[] args)
    {
        var consoleReporter = new ConsoleReporter(false);
        var logger = new DomainReporter(consoleReporter);
        var systemReporter = logger.NewReporter("system");

        VideoConfig? config;
        try
        {
            config = new ConfigLoader<VideoConfig>().Load(args);
        }
        catch (Exception ex)
        {
            systemReporter.Error($"Failed to load configuration: {ex.Message}");
            return ExitConfigError;
        }

        consoleReporter.UseColors = config.ConsoleColorEnabled;

        // Check required parameters
        if (string.IsNullOrEmpty(config.OutputFile) ||
            string.IsNullOrEmpty(config.FfmpegPath) ||
            string.IsNullOrEmpty(config.ScriptFile))
        {
            systemReporter.Error("Missing required parameters: output-file, ffmpeg-path, script");
            return ExitMissingParameters;
        }

        // Check FFmpeg executable
        if (!File.Exists(config.FfmpegPath))
        {
            systemReporter.Error($"FFmpeg not found at {config.FfmpegPath}");
            return ExitFileNotFound;
        }

        // Check output file path
        if (!PathChecking.CanCreateFileAtPath(config.OutputFile))
        {
            systemReporter.Error($"Cannot create output file: {config.OutputFile}");
            return ExitFileNotFound;
        }

        // Check script file exists
        if (!File.Exists(config.ScriptFile))
        {
            systemReporter.Error($"Script file not found: {config.ScriptFile}");
            return ExitFileNotFound;
        }

        try
        {
            VideoFormat format = new VideoFormat(
                config.Width,
                config.Height,
                config.FPS,
                PixelFormat.RGB
            );

            using var videoWriter = new FfmpegVideoWriter(
                format,
                config.OutputFile,
                config.FfmpegPath,
                config.FfmpegHardwareAcceleration
            );
            if (config.FfmpegEcho) logger.Add("ffmpeg", videoWriter);

            var renderProbe = logger.Add("renderer", new RenderLoggingProbe());
            var target = new RawRenderTarget(videoWriter, renderProbe, config.FrameBufferMaxSizeBytes);
            var scene = new Scene(target);

            var invoker = new ScriptInvoker<Scene>(
                scene,
                imports: new[] {
                    "System",
                    "System.Math",
                    "Vidmake.src",
                    "Vidmake.src.scene",
                    "Vidmake.src.scene.elements",
                    "Vidmake.src.positioning"
                },
                references: new[] {
                    typeof(Scene).Assembly,
                    typeof(Vidmake.src.scene.elements.Rectangle).Assembly,
                    typeof(Pixel).Assembly,
                    typeof(Plot2D).Assembly
                }
            );
            logger.Add("script", invoker);

            invoker.Execute(File.ReadAllText(config.ScriptFile));
        }
        catch (FileNotFoundException ex)
        {
            systemReporter.Error($"File not found: {ex.Message}");
            return ExitFileNotFound;
        }
        catch (InvalidOperationException ex)
        {
            systemReporter.Error($"Invalid operation: {ex.Message}");
            return ExitInvalidOperation;
        }
        catch (ArgumentOutOfRangeException ex)
        {
            systemReporter.Error($"Argument out of range: {ex.Message}");
            return ExitInvalidOperation;
        }
        catch (OverflowException ex)
        {
            systemReporter.Error($"Argument overflowed: {ex.Message}");
            return ExitOverflow;
        }
        catch (Exception ex)
        {
            systemReporter.Error($"Unexpected exception: {ex.Message}");
            return ExitUnexpected;
        }

        return ExitSuccess;
    }
}


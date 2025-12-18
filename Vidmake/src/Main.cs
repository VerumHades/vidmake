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
    private const int ExitError = 5;

    static int Main(string[] args)
    {
        VideoConfig? config;
        try
        {
            config = new ConfigLoader<VideoConfig>().Load(args);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to load configuration: {ex.Message}");
            return ExitConfigError;
        }

        if (string.IsNullOrEmpty(config.OutputFile) ||
            string.IsNullOrEmpty(config.FfmpegPath) ||
            string.IsNullOrEmpty(config.ScriptFile))
        {
            Console.Error.WriteLine("Missing required parameters: output-file, ffmpeg-path, script");
            return ExitMissingParameters;
        }

        // Check FFmpeg executable
        if (!File.Exists(config.FfmpegPath))
        {
            Console.Error.WriteLine($"FFmpeg not found at {config.FfmpegPath}");
            return ExitFileNotFound;
        }

        // Check output file path
        if (!PathChecking.CanCreateFileAtPath(config.OutputFile))
        {
            Console.Error.WriteLine($"Cannot create output file: {config.OutputFile}");
            return ExitInvalidOperation;
        }

        // Check script file exists
        if (!File.Exists(config.ScriptFile))
        {
            Console.Error.WriteLine($"Script file not found: {config.ScriptFile}");
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

            var target = new RawRenderTarget(videoWriter, new RenderConsoleLoggingProbe(), config.FrameBufferMaxSizeBytes);
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

            invoker.Execute(File.ReadAllText(config.ScriptFile));
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            return ExitError;
        }

        return ExitSuccess;
    }
}


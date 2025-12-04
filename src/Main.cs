
using System.CommandLine;
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
    static void Main(string[] args)
    {
        var config = ConfigLoader<VideoConfig>.Load(args);

        if (string.IsNullOrEmpty(config.OutputFile) ||
            string.IsNullOrEmpty(config.FfmpegPath) ||
            string.IsNullOrEmpty(config.ScriptFile))
        {
            Console.WriteLine("Missing required parameters: output-file, ffmpeg-path, script");
            return;
        }

        if (!File.Exists(config.FfmpegPath))
        {
            Console.WriteLine($"FFmpeg not found at {config.FfmpegPath}");
            return;
        }

        if (!File.Exists(config.ScriptFile))
        {
            Console.WriteLine($"Script file not found: {config.ScriptFile}");
            return;
        }

        var logger = new DomainReporter(
            new ConsoleReporter(config.ConsoleColorEnabled)
        );
        var systemReporter = logger.NewReporter("system");
        if(!config.FfmpegEcho) logger.Disable("ffmpeg");

        
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
            logger.Add("renderer", videoWriter);

            var renderProbe = logger.Add("ffmpeg", new RenderLoggingProbe());
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
        catch (InvalidOperationException ex)
        {
            systemReporter.Error($"Invalid operation: {ex.Message}");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            systemReporter.Error($"Argument out of range: {ex.Message}");
        }
        catch (OverflowException ex)
        {
            systemReporter.Error($"Argument overflowed: {ex.Message}");
        }
        catch (Exception ex)
        {
            systemReporter.Error($"Unexpected exception: {ex.Message}");
        }
    }
}

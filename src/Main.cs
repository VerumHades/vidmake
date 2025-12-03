
using System.CommandLine;
using System.Reflection;
using System.Text.Json;
using Vidmake.src;
using Vidmake.src.cli;
using Vidmake.src.logging;
using Vidmake.src.rendering;
using Vidmake.src.rendering.writers;
using Vidmake.src.scene;

class Program
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

        VideoFormat format = new VideoFormat(
            config.Width,
            config.Height,
            config.FPS,
            PixelFormat.RGB
        );

        var domainReporter = new DomainReporter(new ConsoleReporter(config.ConsoleColorEnabled));

        var ffmpegReporter = domainReporter.NewReporter("ffmpeg");
        var rendererReporter = domainReporter.NewReporter("renderer");

        if(!config.FfmpegEcho) domainReporter.Disable("ffmpeg");

        using var videoWriter = new FfmpegVideoWriter(
            format,
            config.OutputFile,
            config.FfmpegPath,
            config.FfmpegHardwareAcceleration,
            ffmpegReporter
        );

        var target = new RawRenderTarget(videoWriter, new RenderLoggingProbe(rendererReporter));
        var scene = new Scene(target);

        try
        {
            var invoker = new ScriptInvoker<Scene>(scene);
            invoker.Execute(File.ReadAllText(config.ScriptFile));

            Console.WriteLine("Video rendering complete!");
        }
        catch (Exception)
        {

        }
    }
}

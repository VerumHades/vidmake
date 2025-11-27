using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Text.Json;
using AbstractRendering;
using System.Threading.Tasks;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var root = new RootCommand("VidMake CLI - Manim-like C# renderer");

        var configOption = new Option<string>(
            "--config",
            "Path to a JSON configuration file");

        var widthOption = new Option<int>("--width", () => 1920, "Video width in pixels");
        var heightOption = new Option<int>("--height", () => 1080, "Video height in pixels");
        var fpsOption = new Option<int>("--fps", () => 30, "Frames per second");
        var outputOption = new Option<string>("--output-file", "Output video file path");
        var ffmpegOption = new Option<string>("--ffmpeg-path", "Path to ffmpeg executable");
        var scriptOption = new Option<string>("--script", "Path to C# script to execute");

        root.AddOption(configOption);
        root.AddOption(widthOption);
        root.AddOption(heightOption);
        root.AddOption(fpsOption);
        root.AddOption(outputOption);
        root.AddOption(ffmpegOption);
        root.AddOption(scriptOption);

        root.SetHandler(
            async (configPath, width, height, fps, outputFile, ffmpegPath, scriptPath) =>
            {
                // 1. Load JSON config if provided
                VideoConfig config = new VideoConfig();
                if (!string.IsNullOrEmpty(configPath))
                {
                    if (!File.Exists(configPath))
                    {
                        Console.WriteLine($"Config file not found: {configPath}");
                        return;
                    }

                    string json = await File.ReadAllTextAsync(configPath);
                    config = JsonSerializer.Deserialize<VideoConfig>(json);
                }

                // 2. Apply command-line overrides
                if (width != 1920) config.Width = width;
                if (height != 1080) config.Height = height;
                if (fps != 30) config.FPS = fps;
                if (!string.IsNullOrEmpty(outputFile)) config.OutputFile = outputFile;
                if (!string.IsNullOrEmpty(ffmpegPath)) config.FfmpegPath = ffmpegPath;
                if (!string.IsNullOrEmpty(scriptPath)) config.ScriptFile = scriptPath;

                // 3. Validate required parameters
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

                using var videoWriter = new FfmpegVideoWriter(
                    config.Width,
                    config.Height,
                    config.FPS,
                    PixelFormat.RGB,
                    config.OutputFile,
                    config.FfmpegPath
                );

                var target = new RawRenderTarget(videoWriter);
                var scene = new Scene(target);

                var invoker = new ScriptInvoker<Scene>(scene);
                invoker.Execute(File.ReadAllText(config.ScriptFile));
                videoWriter.FinishVideo();

                Console.WriteLine("Video rendering complete!");
            },
            configOption, widthOption, heightOption, fpsOption, outputOption, ffmpegOption, scriptOption
        );

        return await root.InvokeAsync(args);
    }
}

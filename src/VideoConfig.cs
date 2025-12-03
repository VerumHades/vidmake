using System.Text.Json.Serialization;
using Vidmake.src.cli;

/// <summary>
/// Represents configuration settings for video rendering.
/// Can be loaded from a JSON file and/or overridden via command-line arguments.
/// </summary>
public class VideoConfig
{
    [JsonPropertyName("width")]
    [CliOption("--width", "-w", "Video width in pixels")]
    public int Width { get; set; } = 1920;

    [JsonPropertyName("height")]
    [CliOption("--height", "-h", "Video height in pixels")]
    public int Height { get; set; } = 1080;

    [JsonPropertyName("fps")]
    [CliOption("--fps", "-f", "Frames per second")]
    public int FPS { get; set; } = 30;

    [JsonPropertyName("ffmpegEcho")]
    [CliOption("--ffmpeg-echo", description: "Print ffmpeg output")]
    public bool FfmpegEcho { get; set; } = false;

    [JsonPropertyName("consoleColorEnabled")]
    [CliOption("--console-color", description: "Whether the program should print colored output.")]
    public bool ConsoleColorEnabled { get; set; } = true;

    [JsonPropertyName("ffmpegHardwareAcceleration")]
    [CliOption("--ffmpeg-hardware-acceleration", description: "Whether the program should look for and use a hardware encoder (may fail on some hardware).")]
    public bool FfmpegHardwareAcceleration { get; set; } = true;

    [JsonPropertyName("outputFile")]
    [CliOption("--output", "-o", "Output video file path")]
    public string OutputFile { get; set; }

    [JsonPropertyName("ffmpegPath")]
    [CliOption("--ffmpeg-path", description: "Path to ffmpeg executable")]
    public string FfmpegPath { get; set; }

    [JsonPropertyName("scriptFile")]
    [CliOption("--script", "-s", "Animation script file (.csx)")]
    public string ScriptFile { get; set; }
}
using System.Text.Json.Serialization;

/// <summary>
/// Represents configuration settings for video rendering.
/// Can be loaded from a JSON file and/or overridden via command-line arguments.
/// </summary>
public class VideoConfig
{
    /// <summary>
    /// Width of the video in pixels.
    /// Default value is 1920.
    /// </summary>
    [JsonPropertyName("width")]
    public int Width { get; set; } = 1920;

    /// <summary>
    /// Height of the video in pixels.
    /// Default value is 1080.
    /// </summary>
    [JsonPropertyName("height")]
    public int Height { get; set; } = 1080;

    /// <summary>
    /// Frames per second (FPS) of the output video.
    /// Default value is 30.
    /// </summary>
    [JsonPropertyName("fps")]
    public int FPS { get; set; } = 30;
    
    [JsonPropertyName("ffmpegEcho")]
    public bool FfmpegEcho { get; set; } = false;

    /// <summary>
    /// Output video file path.
    /// This is required and must be specified either in JSON or via command-line.
    /// </summary>
    [JsonPropertyName("outputFile")]
    public string OutputFile { get; set; }

    /// <summary>
    /// Path to the FFmpeg executable used for video encoding.
    /// This is required.
    /// </summary>
    [JsonPropertyName("ffmpegPath")]
    public string FfmpegPath { get; set; }

    /// <summary>
    /// Path to the C# script (.csx) that defines the animation.
    /// This is required.
    /// </summary>
    [JsonPropertyName("scriptFile")]
    public string ScriptFile { get; set; }
}
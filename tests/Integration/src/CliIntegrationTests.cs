using System;
using System.IO;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.Buffered;
using FluentAssertions;
using Xunit;

public class VidmakeCliIntegrationTests
{
    private static readonly string RootDir = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..")
    );

    private static readonly string VidmakeExe = Path.Combine(RootDir, "Vidmake", "bin", "Debug", "net9.0", "Vidmake");
    private static readonly string ReleaseDir = Path.Combine(RootDir, "Vidmake", "template", "release");
    private static readonly string FfmpegDir = Path.Combine(RootDir, "Vidmake", "template");
    private static readonly string OutputDir = Path.Combine(RootDir, "Vidmake", "output");

    private string FindFile(string directory, string[] filenames, bool recursive = true)
    {
        var dirInfo = new DirectoryInfo(directory);
        if (!dirInfo.Exists)
            throw new DirectoryNotFoundException($"Directory does not exist: {directory}");

        // Check top-level files first
        foreach (var name in filenames)
        {
            var candidate = Path.Combine(directory, name);
            if (File.Exists(candidate) && IsAccessible(candidate, name))
                return candidate;
        }

        if (recursive)
        {
            foreach (var file in dirInfo.EnumerateFiles("*", SearchOption.AllDirectories))
            {
                if (filenames.Contains(file.Name) && IsAccessible(file.FullName, file.Name))
                    return file.FullName;
            }
        }

        throw new FileNotFoundException($"No file {string.Join(", ", filenames)} found in {directory}");
    }

    private bool IsAccessible(string path, string name)
    {
        try
        {
            if (Path.GetExtension(name).Equals(".exe", StringComparison.OrdinalIgnoreCase))
            {
                // On Windows, assume .exe is executable if it exists
                return File.Exists(path);
            }
            else
            {
                // Check readable
                using var stream = File.Open(path, FileMode.Open, FileAccess.Read);
                return true;
            }
        }
        catch
        {
            return false;
        }
    }

    private string FindFfmpegExecutable(string ffmpegDir)
    {
        return FindFile(ffmpegDir, new[] { "ffmpeg", "ffmpeg.exe" }, recursive: true);
    }

    private async Task<BufferedCommandResult> RunVidmakeAsync(params string[] args)
    {
        var command = Cli.Wrap(VidmakeExe)
            .WithArguments(args)
            .WithValidation(CliWrap.CommandResultValidation.None); // Do not throw on non-zero exit codes

        return await command.ExecuteBufferedAsync();
    }

    [Fact(DisplayName = "Fails when required parameters are missing")]
    public async Task MissingRequiredParameters_ShouldFail()
    {
        var result = await RunVidmakeAsync(); // No args
        result.ExitCode.Should().Be(2); // ExitMissingParameters
        result.StandardError.Should().Contain("Missing required parameters");
    }

    [Fact(DisplayName = "Fails if output path is empty")]
    public async Task EmptyOutputPath_ShouldFail()
    {
        var ffmpegPath = FindFfmpegExecutable(FfmpegDir);
        var configPath = Path.Combine(ReleaseDir, "config.json");
        var scriptPath = Path.Combine(ReleaseDir, "animation.csx");

        var result = await RunVidmakeAsync(
            "--config", configPath,
            "--script", scriptPath,
            "--ffmpeg-path", ffmpegPath,
            "--output", ""
        );

        result.ExitCode.Should().Be(1);
        result.StandardError.Should().Contain("Option '--output' is required to be a non-null string");
    }

    [Fact(DisplayName = "Fails if ffmpeg path is invalid")]
    public async Task InvalidFfmpegPath_ShouldFail()
    {
        var configPath = Path.Combine(ReleaseDir, "config.json");
        var scriptPath = Path.Combine(ReleaseDir, "animation.csx");

        var result = await RunVidmakeAsync(
            "--config", configPath,
            "--script", scriptPath,
            "--ffmpeg-path", "invalid/path/to/ffmpeg",
            "--output", Path.Combine(OutputDir, "video.mp4")
        );

        result.ExitCode.Should().Be(3); // ExitFileNotFound
        result.StandardError.Should().Contain("FFmpeg not found");
    }

    [Fact(DisplayName = "Fails if config path is invalid")]
    public async Task InvalidConfigPath_ShouldFail()
    {
        var ffmpegPath = FindFfmpegExecutable(FfmpegDir);
        var scriptPath = Path.Combine(ReleaseDir, "animation.csx");

        var result = await RunVidmakeAsync(
            "--config", "invalid/config.json",
            "--script", scriptPath,
            "--ffmpeg-path", ffmpegPath,
            "--output", Path.Combine(OutputDir, "video.mp4")
        );

        result.ExitCode.Should().Be(1); // ExitConfigError
        result.StandardError.Should().Contain("Failed to load configuration");
    }

    [Fact(DisplayName = "Fails if script file does not exist")]
    public async Task ScriptFileNotFound_ShouldFail()
    {
        var ffmpegPath = FindFfmpegExecutable(FfmpegDir);
        var configPath = Path.Combine(ReleaseDir, "config.json");

        var result = await RunVidmakeAsync(
            "--config", configPath,
            "--script", "invalid_script.csx",
            "--ffmpeg-path", ffmpegPath,
            "--output", Path.Combine(OutputDir, "video.mp4")
        );

        result.ExitCode.Should().Be(3); // ExitFileNotFound
        result.StandardError.Should().Contain("Script file not found");
    }

    [Fact(DisplayName = "Succeeds with all valid arguments")]
    public async Task FullArguments_ShouldSucceed()
    {
        var ffmpegPath = FindFfmpegExecutable(FfmpegDir);
        var configPath = Path.Combine(ReleaseDir, "config.json");
        var scriptPath = Path.Combine(ReleaseDir, "animation.csx");
        var outputPath = Path.Combine(OutputDir, "video.mp4");

        if (!Directory.Exists(OutputDir)) Directory.CreateDirectory(OutputDir);

        var result = await RunVidmakeAsync(
            "--config", configPath,
            "--script", scriptPath,
            "--ffmpeg-path", ffmpegPath,
            "--output", outputPath
        );

        result.ExitCode.Should().Be(0);
        result.StandardOutput.Should().Contain("[renderer] Rendering Started");

        File.Exists(outputPath).Should().BeTrue();
    }
}

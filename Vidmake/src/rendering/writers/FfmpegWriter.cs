using System.Diagnostics;
using Vidmake.src.logging;

namespace Vidmake.src.rendering.writers
{
    /// <summary>
    /// Implements IVideoWriter by streaming raw frames to an FFmpeg process.
    /// Supports RGB, RGBA, and Grayscale pixel formats.
    /// </summary>
    public class FfmpegVideoWriter : IVideoWriter, IDisposable
    {
        private readonly string ffmpegPath;
        private readonly Process ffmpegProcess;
        private readonly Stream ffmpegInputStream;

        public VideoFormat Format { get; }

        /// <summary>
        /// Constructor. Starts an FFmpeg process and prepares it to receive raw frames.
        /// </summary>
        public FfmpegVideoWriter(VideoFormat format, string outputFilename, string ffmpegPath, bool hardwareAcceleration)
        {
            Format = format;
            this.ffmpegPath = ffmpegPath;

            string? encoder = hardwareAcceleration ? GetHardwareEncoder() : null;

            ffmpegProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = this.ffmpegPath,
                    Arguments =
                            $"-y " +
                            $"-f rawvideo " +
                            $"-pix_fmt {GetFormatString(format.PixelFormat)} " +
                            $"-s:v {format.Width}x{format.Height} " +
                            $"-r {format.FPS} " +
                            $"-i pipe:0 " +
                            (encoder != null ? $"-c:v {encoder} " : "") +
                            $"-preset fast " +
                            $"-pix_fmt yuv420p " +
                            $"\"{outputFilename}\"",
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardError = false,
                    RedirectStandardOutput = false,
                    CreateNoWindow = true
                }
            };

            try
            {
                ffmpegProcess.Start();
                ffmpegInputStream = ffmpegProcess.StandardInput.BaseStream;
            }
            catch (FileNotFoundException ex)
            {
                throw new FileNotFoundException($"FFmpeg executable not found at path '{ffmpegPath}': {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"Invalid process start configuration: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected exception starting FFmpeg: {ex.Message}");
            }
        }

        private string? GetHardwareEncoder()
        {
            string[] knownHwEncoders = new string[]
            {
                "h264_nvenc",   // NVIDIA
                "hevc_nvenc",   // NVIDIA
                "h264_qsv",     // Intel QuickSync
                "hevc_qsv",     // Intel QuickSync
                "h264_vaapi",   // VAAPI (Linux)
                "hevc_vaapi",   // VAAPI (Linux)
                "h264_videotoolbox", // macOS
                "hevc_videotoolbox"  // macOS
            };

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = ffmpegPath,
                    Arguments = "-encoders",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process? process = Process.Start(psi))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    // Check if any known encoder is in the output
                    foreach (string encoder in knownHwEncoders)
                    {
                        if (output.Contains(encoder))
                            return encoder;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking for ffmpeg hardware acceleration drivers: " + ex.Message);
            }

            return null; // none found
        }

        /// <summary>
        /// Maps PixelFormat enum to FFmpeg raw video format string.
        /// </summary>
        private string GetFormatString(PixelFormat pixelFormat)
        {
            return pixelFormat switch
            {
                PixelFormat.Grayscale => "gray",   // 8-bit single channel
                PixelFormat.RGB => "rgb24",        // 3 bytes per pixel
                PixelFormat.RGBA => "rgba",        // 4 bytes per pixel
                _ => throw new ArgumentOutOfRangeException(nameof(pixelFormat))
            };
        }

        /// <summary>
        /// Writes a sequence of raw frames to FFmpeg.
        /// </summary>
        /// <param name="bytes">Byte array containing frame data.</param>
        /// <param name="frameCount">Number of frames in the array.</param>
        public void Write(byte[] bytes, int frameCount)
        {
            int size = frameCount * Format.FrameSizeInBytes;
            if (bytes.Length < size)
                throw new InvalidDataException("Invalid amount of frame data.");

            try
            {
                ffmpegInputStream.Write(bytes, 0, size);
            }
            catch (ObjectDisposedException ex)
            {
                throw new ObjectDisposedException($"Cannot write to FFmpeg process, stream disposed: {ex.Message}");
            }
            catch (IOException ex)
            {
                throw new IOException($"I/O error writing to FFmpeg: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected error writing to FFmpeg: {ex.Message}");
            }
        }

        /// <summary>
        /// Flushes any buffered frame data to FFmpeg.
        /// </summary>
        public void Flush()
        {
            try
            {
                ffmpegInputStream?.Flush();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error flushing FFmpeg input: {ex.Message}");
            }
        }

        public void Dispose()
        {
            try
            {
                ffmpegInputStream?.Flush();
                ffmpegInputStream?.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error flushing/closing FFmpeg input: {ex.Message}");
            }

            try
            {
                if (!ffmpegProcess.HasExited)
                {
                    ffmpegProcess.WaitForExit();
                }

                if (ffmpegProcess.ExitCode != 0)
                {
                    Console.WriteLine($"FFmpeg exited with code {ffmpegProcess.ExitCode}.");
                }
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error checking FFmpeg exit status: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error during FFmpeg shutdown: {ex.Message}");
            }
        }

    }
}

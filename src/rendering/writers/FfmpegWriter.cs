using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AbstractRendering
{
    /// <summary>
    /// Implements IVideoWriter by streaming raw frames to an FFmpeg process.
    /// Supports RGB, RGBA, and Grayscale pixel formats.
    /// </summary>
    public class FfmpegVideoWriter : IVideoWriter, IDisposable
    {
        private readonly string ffmpegPath;         // Path to FFmpeg executable
        private readonly Process ffmpegProcess;     // FFmpeg process
        private readonly Stream ffmpegInputStream;  // Standard input stream for writing frames
        private bool disposed = false;              // Tracks disposal status

        /// <summary>Pixel format of the video frames.</summary>
        public PixelFormat PixelFormat { get; private set; }

        /// <summary>Frames per second.</summary>
        public int FPS { get; private set; }

        /// <summary>Video width in pixels.</summary>
        public int Width { get; private set; }

        /// <summary>Video height in pixels.</summary>
        public int Height { get; private set; }

        /// <summary>Size of one frame in bytes.</summary>
        public int FrameSizeInBytes { get; private set; }

        /// <summary>
        /// Constructor. Starts an FFmpeg process and prepares it to receive raw frames.
        /// </summary>
        public FfmpegVideoWriter(int width, int height, int targetFPS, PixelFormat pixelFormat, string outputFilename, string ffmpegPath)
        {
            FPS = targetFPS;
            PixelFormat = pixelFormat;
            Width = width;
            Height = height;
            FrameSizeInBytes = Width * Height * (int)PixelFormat;

            this.ffmpegPath = ffmpegPath;
            if (!File.Exists(ffmpegPath))
                throw new FileNotFoundException("FFmpeg executable not found", ffmpegPath);

            ffmpegProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = this.ffmpegPath,
                    Arguments = $"-y -f rawvideo -pix_fmt {GetFormatString(PixelFormat)} -s {width}x{height} -r {targetFPS} -i - -c:v libx264 -pix_fmt yuv420p {outputFilename}",
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            ffmpegProcess.Start();
            ffmpegInputStream = ffmpegProcess.StandardInput.BaseStream;
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
        /// Finalizes the video by flushing and closing the FFmpeg process.
        /// </summary>
        public void FinishVideo()
        {
            if (disposed) return;

            ffmpegInputStream.Flush();
            ffmpegInputStream.Close();

            string errors = ffmpegProcess.StandardError.ReadToEnd();
            ffmpegProcess.WaitForExit();

            if (ffmpegProcess.ExitCode != 0)
                throw new Exception($"FFmpeg exited with code {ffmpegProcess.ExitCode}: {errors}");

            disposed = true;
        }

        /// <summary>
        /// Disposes the video writer and ensures the video is finalized.
        /// </summary>
        public void Dispose()
        {
            if (!disposed)
            {
                FinishVideo();
            }
        }

        /// <summary>
        /// Writes a sequence of raw frames to FFmpeg.
        /// </summary>
        /// <param name="bytes">Byte array containing frame data.</param>
        /// <param name="frameCount">Number of frames in the array.</param>
        public void Write(byte[] bytes, int frameCount)
        {
            int size = frameCount * FrameSizeInBytes;
            if (bytes.Length < size)
                throw new InvalidDataException("Invalid amount of frame data.");

            ffmpegInputStream.Write(bytes, 0, size);
        }

        /// <summary>
        /// Flushes any buffered frame data to FFmpeg.
        /// </summary>
        public void Flush()
        {
            ffmpegInputStream.Flush();
        }
    }
}

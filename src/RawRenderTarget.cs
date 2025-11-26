using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using AbstractRendering;

namespace RawRendering
{
    public class RawRenderTarget : RenderTarget
    {
        private const int BytesPerPixel = 4;
        private readonly int frameSizeBytes;
        private readonly int WidthValue;
        private readonly int HeightValue;
        private readonly int TargetFPSValue;

        // Output file path stored in the render target
        private byte[] framesBuffer;
        private int framesBufferSize = 0;
        private int totalFrames = 0;

        private string ffmpegPath;


        public RawRenderTarget(int width, int height, int targetFPS)
        {
            ffmpegPath = Path.Combine(AppContext.BaseDirectory, "ffmpeg", "win-x86", "ffmpeg.exe");
            if (!File.Exists(ffmpegPath))
                throw new FileNotFoundException("FFmpeg executable not found", ffmpegPath);

            WidthValue = width;
            HeightValue = height;
            TargetFPSValue = targetFPS;
            frameSizeBytes = width * height * BytesPerPixel;
        }

        public override int Width => WidthValue;
        public override int Height => HeightValue;
        public override int TargetFPS => TargetFPSValue;

        public override void AddElementFrames(IReadOnlyList<Element> elements, int frameCount)
        {
            int bufferSize = frameSizeBytes * (totalFrames + frameCount);
            if (framesBufferSize < bufferSize)
            {
                Array.Resize(ref framesBuffer, bufferSize);
                framesBufferSize = bufferSize;
            }

            Parallel.For(0, frameCount, i =>
            {
                float animationPercentage = i / (float)frameCount;
                RenderFrame(framesBuffer.AsSpan(totalFrames * frameSizeBytes + i * frameSizeBytes, frameSizeBytes), elements, animationPercentage);
            });

            totalFrames += frameCount;
        }

        private void RenderFrame(Span<byte> target, IReadOnlyList<Element> elements, float animationPercentage)
        {
            foreach (var element in elements)
            {
                var transform = element.GetInterpolated(element.AnimationInterpolator, animationPercentage);
                var area = new DrawableArea(target, Width, transform.X, transform.Y, transform.Width, transform.Height);
                element.Render(ref area, animationPercentage);
            }
        }

        // Method to write buffered frames to a video using FFmpeg
        public void SaveToVideo(string outputFilePath)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ffmpegPath,
                    Arguments = $"-y -f rawvideo -pix_fmt rgba -s {Width}x{Height} -r {TargetFPS} -i - -c:v libx264 -pix_fmt yuv420p \"{outputFilePath}\"",
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardError = true
                }
            };

            process.Start();

            int offset = 0;
            for (int i = 0; i < totalFrames; i++)
            {
                process.StandardInput.BaseStream.Write(framesBuffer, offset, frameSizeBytes);
                offset += frameSizeBytes;
            }

            process.StandardInput.BaseStream.Flush();
            process.StandardInput.Close();

            // Optional: capture FFmpeg errors
            string errors = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
                throw new Exception($"FFmpeg exited with code {process.ExitCode}: {errors}");
        }
    }
}
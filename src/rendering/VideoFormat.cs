using System;

namespace Vidmake.src.rendering
{
    public class VideoFormat
    {
        public int FrameSizeInBytes { get; }
        public int Width { get; }
        public int Height { get; }
        public int FPS { get; }
        public PixelFormat PixelFormat { get; }

        public VideoFormat(int width, int height, int targetFPS, PixelFormat pixelFormat)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), width,
                    "Width must be greater than zero.");

            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height), height,
                    "Height must be greater than zero.");

            if (targetFPS <= 0)
                throw new ArgumentOutOfRangeException(nameof(targetFPS), targetFPS,
                    "FPS must be greater than zero.");

            if (!Enum.IsDefined(typeof(PixelFormat), pixelFormat))
                throw new ArgumentException(
                    $"Invalid pixel format value: {pixelFormat}", nameof(pixelFormat));


            Width = width;
            Height = height;
            FPS = targetFPS;
            PixelFormat = pixelFormat;

            try
            {
                checked
                {
                    FrameSizeInBytes = Width * Height * (int)PixelFormat;
                }
            }
            catch (OverflowException)
            {
                throw new OverflowException(
                    $"Frame size calculation overflowed. The combination of " +
                    $"width={width}, height={height}, pixelFormat={pixelFormat} " +
                    $"produced a size too large for a 32-bit integer.");
            }
        }
    }
}
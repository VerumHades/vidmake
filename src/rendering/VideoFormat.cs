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
            Width = width;
            Height = height;
            FPS = targetFPS;
            PixelFormat = pixelFormat;
            FrameSizeInBytes = Width * Height * (int)PixelFormat;
        }
    }
}
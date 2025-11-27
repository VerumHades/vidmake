namespace AbstractRendering
{
    /// <summary>
    /// Specifies the pixel format for video frames.
    /// The enum values correspond to the number of bytes per pixel.
    /// </summary>
    public enum PixelFormat
    {
        /// <summary>8-bit grayscale (1 byte per pixel).</summary>
        Grayscale = 1,

        /// <summary>24-bit RGB (3 bytes per pixel).</summary>
        RGB = 3,

        /// <summary>32-bit RGBA (4 bytes per pixel, includes alpha channel).</summary>
        RGBA = 4
    };

    /// <summary>
    /// Represents a video writer that accepts raw frame data and writes it to a video format.
    /// </summary>
    public interface IVideoWriter
    {
        /// <summary>Pixel format of the video frames.</summary>
        public PixelFormat PixelFormat { get; }

        /// <summary>Frames per second of the output video.</summary>
        public int FPS { get; }

        /// <summary>Width of the video in pixels.</summary>
        public int Width { get; }

        /// <summary>Height of the video in pixels.</summary>
        public int Height { get; }

        /// <summary>Size of a single frame in bytes (Width * Height * BytesPerPixel).</summary>
        public int FrameSizeInBytes { get; }

        /// <summary>
        /// Writes one or more raw frames to the video output.
        /// </summary>
        /// <param name="frameData">Byte array containing frame data.</param>
        /// <param name="frameCount">Number of frames in the array.</param>
        public void Write(byte[] frameData, int frameCount);

        /// <summary>
        /// Flushes any buffered frame data to the output.
        /// </summary>
        public void Flush();
    }
}
namespace Vidmake.src.rendering.writers
{
    /// <summary>
    /// Represents a video writer that accepts raw frame data and writes it to a video format.
    /// </summary>
    public interface IVideoWriter
    {
        /// <summary>Pixel format of the video.</summary>
        public VideoFormat Format {get;}

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
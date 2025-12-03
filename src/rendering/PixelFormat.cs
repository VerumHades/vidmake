namespace Vidmake.src.rendering
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
}
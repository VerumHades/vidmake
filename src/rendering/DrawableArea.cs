namespace AbstractRendering
{
    /// <summary>
    /// Represents a rectangular area of pixels within a larger image buffer.
    /// Provides functions to set pixels, rows, columns, or fill the area.
    /// Supports multiple pixel formats: Grayscale, RGB, RGBA.
    /// </summary>
    public readonly ref struct DrawableArea
    {
        // Underlying pixel buffer for the entire image
        private readonly Span<byte> buffer;

        // Full width of the image (used for indexing)
        private readonly int fullWidth;

        // Offsets for the top-left corner of this drawable subarea
        private readonly int offsetX;
        private readonly int offsetY;

        /// <summary>Width of the drawable area.</summary>
        public int Width { get; }

        /// <summary>Height of the drawable area.</summary>
        public int Height { get; }

        /// <summary>Pixel format of the drawable area (determines bytes per pixel).</summary>
        public PixelFormat Format { get; }

        /// <summary>Number of bytes per pixel based on the format.</summary>
        public int BytesPerPixel => (int)Format;

        /// <summary>
        /// Creates a DrawableArea representing a subarea of a larger image buffer.
        /// </summary>
        /// <param name="fullImageBuffer">Full image buffer.</param>
        /// <param name="fullImageWidth">Width of the full image.</param>
        /// <param name="x">X offset of the subarea.</param>
        /// <param name="y">Y offset of the subarea.</param>
        /// <param name="width">Width of the subarea.</param>
        /// <param name="height">Height of the subarea.</param>
        /// <param name="format">Pixel format of the area.</param>
        public DrawableArea(
            Span<byte> fullImageBuffer,
            int fullImageWidth,
            int x, int y,
            int width, int height,
            PixelFormat format)
        {
            if ((uint)x + (uint)width > (uint)fullImageWidth)
                throw new ArgumentOutOfRangeException(nameof(width));

            int bpp = (int)format;

            int requiredLength = fullImageWidth * (y + height) * bpp;
            if (fullImageBuffer.Length < requiredLength)
                throw new ArgumentException("Buffer too small for the specified subarea.");

            fullWidth = fullImageWidth;
            offsetX = x;
            offsetY = y;
            Width = width;
            Height = height;
            Format = format;

            // Slice the buffer to the subarea starting point
            buffer = fullImageBuffer.Slice(
                (y * fullWidth + x) * bpp,
                height * fullWidth * bpp);
        }

        /// <summary>
        /// Converts (x, y) coordinates within the area to a byte index in the buffer.
        /// </summary>
        private int GetIndex(int x, int y)
        {
            if ((uint)x >= (uint)Width || (uint)y >= (uint)Height)
                throw new ArgumentOutOfRangeException();

            return y * fullWidth * BytesPerPixel + x * BytesPerPixel;
        }

        /// <summary>
        /// Sets a single pixel at (x, y) with the given color.
        /// Handles Grayscale, RGB, and RGBA formats.
        /// </summary>
        public void SetPixel(int x, int y, Pixel pixel)
        {
            int idx = GetIndex(x, y);
            switch (Format)
            {
                case PixelFormat.Grayscale:
                    buffer[idx] = pixel.R; // Use R as luminance
                    break;
                case PixelFormat.RGB:
                    buffer[idx]     = pixel.R;
                    buffer[idx + 1] = pixel.G;
                    buffer[idx + 2] = pixel.B;
                    break;
                case PixelFormat.RGBA:
                    buffer[idx]     = pixel.R;
                    buffer[idx + 1] = pixel.G;
                    buffer[idx + 2] = pixel.B;
                    buffer[idx + 3] = pixel.A;
                    break;
            }
        }

        /// <summary>
        /// Sets all pixels in a specific row to the given color.
        /// </summary>
        public void SetRow(int y, Pixel pixel)
        {
            if ((uint)y >= (uint)Height)
                throw new ArgumentOutOfRangeException();

            int bpp = BytesPerPixel;
            Span<byte> row = buffer.Slice(y * fullWidth * bpp, Width * bpp);

            for (int x = 0; x < Width; x++)
            {
                int idx = x * bpp;
                switch (Format)
                {
                    case PixelFormat.Grayscale: row[idx] = pixel.R; break;
                    case PixelFormat.RGB:
                        row[idx]     = pixel.R;
                        row[idx + 1] = pixel.G;
                        row[idx + 2] = pixel.B;
                        break;
                    case PixelFormat.RGBA:
                        row[idx]     = pixel.R;
                        row[idx + 1] = pixel.G;
                        row[idx + 2] = pixel.B;
                        row[idx + 3] = pixel.A;
                        break;
                }
            }
        }

        /// <summary>
        /// Sets all pixels in a specific column to the given color.
        /// </summary>
        public void SetColumn(int x, Pixel pixel)
        {
            if ((uint)x >= (uint)Width)
                throw new ArgumentOutOfRangeException();

            for (int y = 0; y < Height; y++)
                SetPixel(x, y, pixel);
        }

        /// <summary>
        /// Fills the entire drawable area with a single color.
        /// </summary>
        public void Fill(Pixel pixel)
        {
            for (int y = 0; y < Height; y++)
                SetRow(y, pixel);
        }
    }
}

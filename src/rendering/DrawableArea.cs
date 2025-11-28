namespace AbstractRendering
{
    /// <summary>
    /// Represents a drawable subrectangle within a full image buffer.
    /// Both the subarea offset and local pixel coordinates may be negative.
    /// All writes are clipped against the subarea and the full image.
    /// </summary>
    public readonly ref struct DrawableArea
    {
        private readonly Span<byte> buffer;
        private readonly int fullWidth;
        private readonly int fullHeight;

        private readonly int offsetX;
        private readonly int offsetY;

        public int Width { get; }
        public int Height { get; }
        public PixelFormat Format { get; }

        public int BytesPerPixel => (int)Format;

        public DrawableArea(
            Span<byte> fullImageBuffer,
            int imageWidth,
            int imageHeight,
            int subX,
            int subY,
            int subWidth,
            int subHeight,
            PixelFormat format)
        {
            if (imageWidth <= 0 || imageHeight <= 0)
                throw new ArgumentOutOfRangeException(nameof(imageWidth));

            if (subWidth < 0 || subHeight < 0)
                throw new ArgumentOutOfRangeException(nameof(subWidth));

            int requiredLength = imageWidth * imageHeight * (int)format;
            if (fullImageBuffer.Length < requiredLength)
                throw new ArgumentOutOfRangeException(nameof(fullImageBuffer),
                    "Image buffer is too small.");

            fullWidth = imageWidth;
            fullHeight = imageHeight;
            buffer = fullImageBuffer;

            offsetX = subX;
            offsetY = subY;

            Width = subWidth;
            Height = subHeight;

            Format = format;
        }

        private int GetAbsoluteIndex(int x, int y)
            => (y * fullWidth + x) * BytesPerPixel;

        /// <summary>
        /// Sets a pixel using local coordinates which may be negative.
        /// Writes only if the resulting position lies within both the subarea and full image.
        /// </summary>
        public void SetPixel(int x, int y, Pixel pixel)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                return;

            int imageX = x + offsetX;
            int imageY = y + offsetY;

            if (imageX < 0 || imageX >= fullWidth || imageY < 0 || imageY >= fullHeight)
                return;

            int idx = GetAbsoluteIndex(imageX, imageY);

            switch (Format)
            {
                case PixelFormat.Grayscale:
                    buffer[idx] = pixel.R;
                    break;

                case PixelFormat.RGB:
                    buffer[idx] = pixel.R;
                    buffer[idx + 1] = pixel.G;
                    buffer[idx + 2] = pixel.B;
                    break;

                case PixelFormat.RGBA:
                    buffer[idx] = pixel.R;
                    buffer[idx + 1] = pixel.G;
                    buffer[idx + 2] = pixel.B;
                    buffer[idx + 3] = pixel.A;
                    break;
            }
        }

        /// <summary>
        /// Sets an entire row of the subarea (local y coordinate may be negative).
        /// </summary>
        public void SetRow(int y, Pixel pixel)
        {
            if (y < 0 || y >= Height)
                return;

            for (int x = 0; x < Width; x++)
                SetPixel(x, y, pixel);
        }

        /// <summary>
        /// Sets an entire column of the subarea (local x coordinate may be negative).
        /// </summary>
        public void SetColumn(int x, Pixel pixel)
        {
            if (x < 0 || x >= Width)
                return;

            for (int y = 0; y < Height; y++)
                SetPixel(x, y, pixel);
        }

        /// <summary>
        /// Fills the subarea with a color, respecting clipping rules.
        /// </summary>
        public void Fill(Pixel pixel)
        {
            for (int y = 0; y < Height; y++)
                SetRow(y, pixel);
        }
    }
}

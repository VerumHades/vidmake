namespace AbstractRendering
{
    public readonly ref struct DrawableArea
{
    private readonly Span<byte> buffer;
    private readonly int fullWidth; // width of the entire image for indexing
    private readonly int offsetX;
    private readonly int offsetY;

    public int Width { get; }
    public int Height { get; }

    /// <summary>
    /// Creates a DrawableArea representing a subarea of a larger image buffer.
    /// </summary>
    /// <param name="fullImageBuffer">The span of the full image (RGBA)</param>
    /// <param name="fullImageWidth">Width of the full image</param>
    /// <param name="x">X offset of the subarea</param>
    /// <param name="y">Y offset of the subarea</param>
    /// <param name="width">Width of the subarea</param>
    /// <param name="height">Height of the subarea</param>
    public DrawableArea(Span<byte> fullImageBuffer, int fullImageWidth, int x, int y, int width, int height)
    {
        if ((uint)x + (uint)width > (uint)fullImageWidth)
            throw new ArgumentOutOfRangeException(nameof(width));
        if ((uint)y + (uint)height > int.MaxValue) // optionally check total height bounds
            throw new ArgumentOutOfRangeException(nameof(height));
        if (fullImageBuffer.Length < fullImageWidth * (y + height) * 4)
            throw new ArgumentException("Buffer too small for the specified subarea.");

        this.fullWidth = fullImageWidth;
        this.offsetX = x;
        this.offsetY = y;
        this.Width = width;
        this.Height = height;

        // buffer starts at the top-left corner of the subarea
        buffer = fullImageBuffer.Slice((y * fullWidth + x) * 4, height * fullWidth * 4);
    }

    private int GetIndex(int x, int y)
    {
        if ((uint)x >= (uint)Width || (uint)y >= (uint)Height)
            throw new ArgumentOutOfRangeException();
        return y * fullWidth * 4 + x * 4;
    }

    public void SetPixel(int x, int y, Pixel pixel)
    {
        int idx = GetIndex(x, y);
        buffer[idx]     = pixel.R;
        buffer[idx + 1] = pixel.G;
        buffer[idx + 2] = pixel.B;
        buffer[idx + 3] = pixel.A;
    }

    public void SetRow(int y, Pixel pixel)
    {
        if ((uint)y >= (uint)Height)
            throw new ArgumentOutOfRangeException();

        Span<byte> row = buffer.Slice(y * fullWidth * 4, Width * 4);
        for (int x = 0; x < Width; x++)
        {
            int idx = x * 4;
            row[idx]     = pixel.R;
            row[idx + 1] = pixel.G;
            row[idx + 2] = pixel.B;
            row[idx + 3] = pixel.A;
        }
    }

    public void SetColumn(int x, Pixel pixel)
    {
        if ((uint)x >= (uint)Width)
            throw new ArgumentOutOfRangeException();

        for (int y = 0; y < Height; y++)
        {
            int idx = GetIndex(x, y);
            buffer[idx]     = pixel.R;
            buffer[idx + 1] = pixel.G;
            buffer[idx + 2] = pixel.B;
            buffer[idx + 3] = pixel.A;
        }
    }

    public void Fill(Pixel pixel)
    {
        for (int y = 0; y < Height; y++)
        {
            Span<byte> row = buffer.Slice(y * fullWidth * 4, Width * 4);
            for (int x = 0; x < Width; x++)
            {
                int idx = x * 4;
                row[idx]     = pixel.R;
                row[idx + 1] = pixel.G;
                row[idx + 2] = pixel.B;
                row[idx + 3] = pixel.A;
            }
        }
    }
}
}

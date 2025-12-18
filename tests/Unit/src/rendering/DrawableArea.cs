using System;
using Vidmake.src.rendering;
using Xunit;

public class DrawableAreaExtremeTests
{
    private Pixel MakePixel(byte value = 128) => new Pixel(value, value, value, value);

    /// <summary>
    /// Asserts that the buffer at the given x, y coordinates matches the pixel for the specified format.
    /// </summary>
    private void AssertPixelAt(byte[] buffer, int width, int x, int y, Pixel pixel, PixelFormat format)
    {
        int size = (int)format;
        int idx = (y * width + x) * size;

        Assert.Equal(pixel.R, buffer[idx]);

        if (format == PixelFormat.RGB || format == PixelFormat.RGBA)
        {
            Assert.Equal(pixel.G, buffer[idx + 1]);
            Assert.Equal(pixel.B, buffer[idx + 2]);
        }

        if (format == PixelFormat.RGBA)
        {
            Assert.Equal(pixel.A, buffer[idx + 3]);
        }
    }

    [Theory]
    [InlineData(PixelFormat.Grayscale)]
    [InlineData(PixelFormat.RGB)]
    [InlineData(PixelFormat.RGBA)]
    public void NegativeOffsets_DoNotWriteOutside(PixelFormat format)
    {
        var buffer = new byte[10 * 10 * (int)format];
        var area = new DrawableArea(buffer, 10, 10, -5, -5, 10, 10, format);

        var fillPixel = MakePixel(255);

        for (int y = -5; y < 15; y++)
        {
            for (int x = -5; x < 15; x++)
                area.SetPixel(x, y, fillPixel);
        }

        for (int y = 0; y < 10; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                bool inWrittenArea = x < 5 && y < 5;
                var expectedPixel = inWrittenArea ? fillPixel : MakePixel(0);
                AssertPixelAt(buffer, 10, x, y, expectedPixel, format);
            }
        }
    }

    [Theory]
    [InlineData(PixelFormat.Grayscale)]
    [InlineData(PixelFormat.RGB)]
    [InlineData(PixelFormat.RGBA)]
    public void MinimalSize1x1_WritesCorrectly(PixelFormat format)
    {
        var buffer = new byte[(int)format];
        var area = new DrawableArea(buffer, 1, 1, 0, 0, 1, 1, format);
        var pixel = MakePixel(200);

        area.SetPixel(0, 0, pixel);

        AssertPixelAt(buffer, 1, 0, 0, pixel, format);
    }

    [Theory]
    [InlineData(PixelFormat.Grayscale)]
    [InlineData(PixelFormat.RGB)]
    [InlineData(PixelFormat.RGBA)]
    public void Fill_ClippingAtEdges(PixelFormat format)
    {
        int size = (int)format;
        var buffer = new byte[4 * 4 * size];
        var pixel = MakePixel(123);

        var area = new DrawableArea(buffer, 4, 4, 2, 2, 4, 4, format);
        area.Fill(pixel);

        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                bool inWrittenArea = x >= 2 && y >= 2;
                var expectedPixel = inWrittenArea ? pixel : MakePixel(0);
                AssertPixelAt(buffer, 4, x, y, expectedPixel, format);
            }
        }
    }

    [Theory]
    [InlineData(PixelFormat.Grayscale)]
    [InlineData(PixelFormat.RGB)]
    [InlineData(PixelFormat.RGBA)]
    public void SetRowAndColumn_NegativeAndClipping(PixelFormat format)
    {
        int size = (int)format;
        var buffer = new byte[5 * 5 * size];
        var pixel = MakePixel(50);
        var emptyPixel = MakePixel(0);
        var area = new DrawableArea(buffer, 5, 5, 0, 0, 5, 5, format);

        area.SetRow(-1, pixel); // Should do nothing
        area.SetColumn(-1, pixel); // Should do nothing
        area.SetRow(4, pixel);
        area.SetColumn(4, pixel);

        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                bool inWrittenArea = y == 4 || x == 4;
                var expectedPixel = inWrittenArea ? pixel : emptyPixel;
                AssertPixelAt(buffer, 5, x, y, expectedPixel, format);
            }
        }
    }

    [Fact]
    public void SubareaExceedsBuffer_ThrowsException()
    {
        var buffer = new byte[5 * 5 * 3]; // RGB
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new DrawableArea(buffer, 10, 10, 0, 0, 10, 10, PixelFormat.RGB));
    }
}

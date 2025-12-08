using System;
using Vidmake.src.rendering;
using Xunit;

public class DrawableAreaExtremeTests
{
    private Pixel MakePixel(byte value = 128) => new Pixel(value, value, value, value);

    [Theory]
    [InlineData(PixelFormat.Grayscale)]
    [InlineData(PixelFormat.RGB)]
    [InlineData(PixelFormat.RGBA)]
    public void NegativeOffsets_DoNotWriteOutside(PixelFormat format)
    {
        var buffer = new byte[10 * 10 * (int)format];
        var area = new DrawableArea(buffer, 10, 10, -5, -5, 10, 10, format);

        // Fill the subarea with a value
        for (int y = -5; y < 15; y++)
        {
            for (int x = -5; x < 15; x++)
                area.SetPixel(x, y, MakePixel(255));
        }

        for (int y = 0; y < 10; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                int idx = (y * 10 + x) * (int)format;
                int imageX = x;
                int imageY = y;
                
                if (imageX >= 0 && imageX < 5 && imageY >= 0 && imageY < 5)
                {
                    Assert.Equal(255, buffer[idx]);
                    if (format == PixelFormat.RGB)
                    {
                        Assert.Equal(255, buffer[idx + 1]);
                        Assert.Equal(255, buffer[idx + 2]);
                    }
                    else if (format == PixelFormat.RGBA)
                    {
                        Assert.Equal(255, buffer[idx + 1]);
                        Assert.Equal(255, buffer[idx + 2]);
                        Assert.Equal(255, buffer[idx + 3]);
                    }
                }
                else
                {
                    Assert.Equal(0, buffer[idx]);
                }
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

        Assert.Equal(pixel.R, buffer[0]);
        if (format == PixelFormat.RGB)
        {
            Assert.Equal(pixel.G, buffer[1]);
            Assert.Equal(pixel.B, buffer[2]);
        }
        else if (format == PixelFormat.RGBA)
        {
            Assert.Equal(pixel.G, buffer[1]);
            Assert.Equal(pixel.B, buffer[2]);
            Assert.Equal(pixel.A, buffer[3]);
        }
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
                int idx = (y * 4 + x) * size;
                bool inWrittenArea = x >= 2 && y >= 2;
                if (inWrittenArea)
                {
                    Assert.Equal(123, buffer[idx]);
                    if (format == PixelFormat.RGB || format == PixelFormat.RGBA)
                    {
                        Assert.Equal(123, buffer[idx + 1]);
                        Assert.Equal(123, buffer[idx + 2]);
                    }
                    if (format == PixelFormat.RGBA)
                        Assert.Equal(123, buffer[idx + 3]);
                }
                else
                {
                    Assert.Equal(0, buffer[idx]);
                    if (format == PixelFormat.RGB || format == PixelFormat.RGBA)
                    {
                        Assert.Equal(0, buffer[idx + 1]);
                        Assert.Equal(0, buffer[idx + 2]);
                    }
                    if (format == PixelFormat.RGBA)
                        Assert.Equal(0, buffer[idx + 3]);
                }
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
        var area = new DrawableArea(buffer, 5, 5, 0, 0, 5, 5, format);

        area.SetRow(-1, pixel); // Should do nothing
        area.SetColumn(-1, pixel); // Should do nothing
        area.SetRow(4, pixel);
        area.SetColumn(4, pixel);

        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                int idx = (y * 5 + x) * size;
                if (y == 4 || x == 4)
                {
                    Assert.Equal(50, buffer[idx]);
                    if (format == PixelFormat.RGB || format == PixelFormat.RGBA)
                    {
                        Assert.Equal(50, buffer[idx + 1]);
                        Assert.Equal(50, buffer[idx + 2]);
                    }
                    if (format == PixelFormat.RGBA)
                        Assert.Equal(50, buffer[idx + 3]);
                }
                else
                {
                    Assert.Equal(0, buffer[idx]);
                    if (format == PixelFormat.RGB || format == PixelFormat.RGBA)
                    {
                        Assert.Equal(0, buffer[idx + 1]);
                        Assert.Equal(0, buffer[idx + 2]);
                    }
                    if (format == PixelFormat.RGBA)
                        Assert.Equal(0, buffer[idx + 3]);
                }
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

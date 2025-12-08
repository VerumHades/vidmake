/// <summary>
/// Represents a single RGBA pixel with 8-bit channels.
/// Immutable once created (readonly struct).
/// May and can be used to represent colors of a lower byte count, that is: RGB (ignores alpha), Grayscale (takes into account only the red channel)
/// </summary>
public readonly struct Pixel
{
    /// <summary>Red channel (0–255).</summary>
    public readonly byte R;

    /// <summary>Green channel (0–255).</summary>
    public readonly byte G;

    /// <summary>Blue channel (0–255).</summary>
    public readonly byte B;

    /// <summary>Alpha channel (0–255). Defaults to 255 (fully opaque).</summary>
    public readonly byte A;

    /// <summary>
    /// Constructor specifying each channel individually.
    /// </summary>
    /// <param name="r">Red (0–255)</param>
    /// <param name="g">Green (0–255)</param>
    /// <param name="b">Blue (0–255)</param>
    /// <param name="a">Alpha (0–255, default 255)</param>
    public Pixel(byte r, byte g, byte b, byte a = 255)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    // Predefined common colors for convenience.
    public static readonly Pixel Black = new(0, 0, 0);
    public static readonly Pixel White = new(255, 255, 255);
    public static readonly Pixel Red = new(255, 0, 0);
    public static readonly Pixel Green = new(0, 255, 0);
    public static readonly Pixel Blue = new(0, 0, 255);
}

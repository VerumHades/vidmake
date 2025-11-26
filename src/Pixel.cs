using System;

public readonly struct Pixel
{
    public readonly byte R;
    public readonly byte G;
    public readonly byte B;
    public readonly byte A;

    // Constructor
    public Pixel(byte r, byte g, byte b, byte a = 255)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    // Convenience: from 32-bit uint
    public Pixel(uint rgba)
    {
        R = (byte)((rgba >> 24) & 0xFF);
        G = (byte)((rgba >> 16) & 0xFF);
        B = (byte)((rgba >> 8) & 0xFF);
        A = (byte)(rgba & 0xFF);
    }

    // Convert to 32-bit uint
    public uint ToUInt32()
    {
        return ((uint)R << 24) | ((uint)G << 16) | ((uint)B << 8) | A;
    }

    // Predefined colors
    public static readonly Pixel Black = new(0, 0, 0);
    public static readonly Pixel White = new(255, 255, 255);
    public static readonly Pixel Red   = new(255, 0, 0);
    public static readonly Pixel Green = new(0, 255, 0);
    public static readonly Pixel Blue  = new(0, 0, 255);
}
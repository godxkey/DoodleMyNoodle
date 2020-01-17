using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public struct UPaintLayer
{
    public NativeArray<Color32> Pixels;
    public int Width;
    public int Height;

    public Color32 this[int index]
    {
        get => Pixels[index];
        set => Pixels[index] = value;
    }
    public Color32 this[int x, int y]
    {
        get => Pixels[GetPixelIndexFromCoordinates(x, y)];
        set => Pixels[GetPixelIndexFromCoordinates(x, y)] = value;
    }
    public Color32 this[in int2 coordinates]
    {
        get => Pixels[GetPixelIndexFromCoordinates(coordinates)];
        set => Pixels[GetPixelIndexFromCoordinates(coordinates)] = value;
    }
    public int PixelCount => Pixels.Length;

    public bool IsValidPixel(int x, int y) =>
        x >= 0 && x < Width &&
        y >= 0 && y < Height;

    public int GetPixelIndexFromCoordinates(in int x, in int y)
    {
        return x + (y * Width);
    }
    public int GetPixelIndexFromCoordinates(in int2 coordinates)
    {
        return GetPixelIndexFromCoordinates(coordinates.x, coordinates.y);
    }
}
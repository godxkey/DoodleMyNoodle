using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

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
    public bool IsValidPixel(in int2 p) => IsValidPixel(p.x, p.y);

    public int GetPixelIndexFromCoordinates(in int x, in int y)
    {
        return x + (y * Width);
    }
    public int GetPixelIndexFromCoordinates(in int2 coordinates)
    {
        return GetPixelIndexFromCoordinates(coordinates.x, coordinates.y);
    }
    public void GetPixelCoordinatesFromIndex(in int index, out int x, out int y)
    {
        x = index % Width;
        y = (index - x) / Width;
    }
    public int2 GetPixelCoordinatesFromIndex(in int index)
    {
        GetPixelCoordinatesFromIndex(index, out int x, out int y);
        return int2(x, y);
    }
}
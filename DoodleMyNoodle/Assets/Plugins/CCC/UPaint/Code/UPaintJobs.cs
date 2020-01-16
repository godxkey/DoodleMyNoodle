using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using UnityEngine;

namespace UPaintJobs
{
    public struct PaintPoint : IJob
    {
        public UPaintLayerAccessor Layer;
        public Color32 Color;
        public Vector2Int Coordinates;

        public void Execute()
        {
            Layer[Coordinates] = Color;
        }
    }
    public struct PaintSquare : IJob
    {
        public UPaintLayerAccessor Layer;
        public Color32 Color;
        public Vector2Int CenterCoordinates;
        public int Width;

        public void Execute()
        {
            int xStart = CenterCoordinates.x - (Width / 2);
            int xEnd = xStart + Width;
            int yStart = CenterCoordinates.y - (Width / 2);
            int yEnd = yStart + Width;

            for (int x = xStart; x < xEnd; x++)
            {
                for (int y = yStart; y < yEnd; y++)
                {
                    if (Layer.IsValidPixel(x, y))
                    {
                        Layer[x, y] = Color;
                    }
                }
            }
        }
    }
    public struct PaintCircle : IJob
    {
        public UPaintLayerAccessor Layer;
        public Color32 Color;
        public Vector2Int CenterCoordinates;
        public int Width;
        public float Gradient01;

        public void Execute()
        {
            int xStart = CenterCoordinates.x - (Width / 2);
            int xEnd = xStart + Width;
            int yStart = CenterCoordinates.y - (Width / 2);
            int yEnd = yStart + Width;

            float edgeSqrMagnitude = (Width / 2) * (Width / 2);

            Vector2 pixelToCircleCenter;
            float sqrMag;
            float blend01;

            for (int x = xStart; x < xEnd; x++)
            {
                for (int y = yStart; y < yEnd; y++)
                {
                    pixelToCircleCenter.x = x - CenterCoordinates.x;
                    pixelToCircleCenter.y = y - CenterCoordinates.y;

                    sqrMag = pixelToCircleCenter.sqrMagnitude;

                    if (Layer.IsValidPixel(x, y) && sqrMag <= edgeSqrMagnitude)
                    {
                        if (Gradient01 > 0.0001)
                        {
                            blend01 =Mathf.Clamp01(((sqrMag / edgeSqrMagnitude) - (1 - Gradient01)) / Gradient01);
                        }
                        else
                        {
                            blend01 = 0;
                        }

                        Layer[x, y] = Color32.Lerp(Color, Layer[x, y], blend01);
                    }
                }
            }
        }
    }

    public struct PaintVector : IJob
    {
        public UPaintLayerAccessor Layer;
        public Color32 Color;
        public Vector2Int Coordinates;

        public void Execute()
        {
            Layer[Coordinates] = Color;
        }
    }

    public struct PaintAllPixels : IJob
    {
        public NativeArray<Color32> Layer;
        public Color32 Color;

        public void Execute()
        {
            for (int i = 0; i < Layer.Length; i++)
            {
                Layer[i] = Color;
            }
        }
    }

    public struct ApplyLayerOneOntoLayerTwo : IJob
    {
        public NativeArray<Color32> LayerOne;
        public NativeArray<Color32> LayerTwo;

        public void Execute()
        {
            for (int i = 0; i < LayerOne.Length; i++)
            {
                LayerTwo[i] = UPaintOperations.AlphaBlendColorOneOntoTwo(LayerOne[i], LayerTwo[i]);
            }
        }
    }
}
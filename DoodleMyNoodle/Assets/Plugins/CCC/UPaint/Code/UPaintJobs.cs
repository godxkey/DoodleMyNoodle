using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace UPaintJobs
{
    [BurstCompile]
    public struct PaintPoint : IJob
    {
        public UPaintLayer Layer;
        public Color32 Color;
        public int2 Coordinates;

        public void Execute()
        {
            Layer[Coordinates] = Color;
        }
    }

    [BurstCompile]
    public struct PaintSquare : IJob
    {
        public UPaintLayer Layer;
        public Color32 Color;
        public int2 CenterCoordinates;
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

    [BurstCompile]
    public struct PaintAllPixels : IJob
    {
        public UPaintLayer Layer;
        public Color32 Color;

        public void Execute()
        {
            for (int i = 0; i < Layer.PixelCount; i++)
            {
                Layer[i] = Color;
            }
        }
    }
    
    [BurstCompile]
    public struct CopyLayerOneToTwo : IJob
    {
        [ReadOnly] public UPaintLayer LayerOne;
        [ReadOnly] public UPaintLayer LayerTwo;

        public void Execute()
        {
            LayerTwo.Height = LayerOne.Height;
            LayerTwo.Width = LayerOne.Width;
            for (int i = 0; i < LayerOne.PixelCount; i++)
            {
                LayerTwo[i] = LayerOne[i];
            }
        }
    }

    [BurstCompile]
    public struct BlendLayerOneOntoLayerTwo : IJob
    {
        [ReadOnly] public UPaintLayer LayerOne;
        public UPaintLayer LayerTwo;

        public void Execute()
        {
            for (int i = 0; i < LayerOne.PixelCount; i++)
            {
                LayerTwo[i] = UPaintOperations.AlphaBlendColorOneOntoTwo(LayerOne[i], LayerTwo[i]);
            }
        }
    }
}
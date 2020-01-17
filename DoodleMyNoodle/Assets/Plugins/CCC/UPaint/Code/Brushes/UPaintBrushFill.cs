using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;
using Unity.Collections;

namespace UPaintBrushes
{
    [System.Serializable]
    public class Fill : IUPaintBursh
    {
        public void OnPress(IUPaintBrushCanvasInterface canvas, in UPaintContext context)
        {
            int2 pixelPosition = int2(round(context.CursorCoordinate));

            canvas.ScheduleNextPaintJob(new FillJob()
            {
                BorderPixels = new NativeArray<int>(canvas.PreviewLayer.PixelCount, Allocator.TempJob),
                StartCoordinates = pixelPosition,
                NewColor = context.Color,
                ReadLayer = canvas.MainLayer,
                WriteLayer = canvas.PreviewLayer
            });

            canvas.ScheduleBlendPreviewOntoMainLayer();
        }

        public void OnHold(IUPaintBrushCanvasInterface canvas, in UPaintContext context)
        {
        }

        public void OnRelease(IUPaintBrushCanvasInterface canvas, in UPaintContext context)
        {
        }

        [BurstCompile]
        public struct FillJob : IJob
        {
            [DeallocateOnJobCompletion] public NativeArray<int> BorderPixels;

            public UPaintLayer WriteLayer;
            [ReadOnly] public UPaintLayer ReadLayer;
            [ReadOnly] public int2 StartCoordinates;
            [ReadOnly] public Color32 NewColor;

            private Color32 _replaceColor;
            private int _borderCount;

            public void Execute()
            {
                _replaceColor = ReadLayer[StartCoordinates];

                if (ReadLayer.IsValidPixel(StartCoordinates))
                    PushBorder(StartCoordinates);

                while (_borderCount > 0)
                {
                    int pixelIndex = PopBorder();
                    WriteLayer[pixelIndex] = NewColor;

                    int2 pixelPosition = ReadLayer.GetPixelCoordinatesFromIndex(pixelIndex);

                    TestPotentialBorder(pixelPosition + int2(0, -1));// down
                    TestPotentialBorder(pixelPosition + int2(0, 1)); // up
                    TestPotentialBorder(pixelPosition + int2(-1, 0));// left
                    TestPotentialBorder(pixelPosition + int2(1, 0)); // right
                }
            }

            bool equal(in Color32 a, in Color32 b)
                => a.r == b.r
                && a.g == b.g
                && a.b == b.b
                && a.a == b.a;

            void TestPotentialBorder(in int2 pixelPos)
            {
                if (WriteLayer.IsValidPixel(pixelPos) &&         // valid pixel ?
                    !equal(WriteLayer[pixelPos], NewColor) &&    // color already filled there ?
                    equal(ReadLayer[pixelPos], _replaceColor))   // corresponds to color we want to replace ?
                {
                    PushBorder(pixelPos);
                }
            }
            void PushBorder(in int2 coordinates)
            {
                PushBorder(ReadLayer.GetPixelIndexFromCoordinates(coordinates));
            }
            void PushBorder(in int index)
            {
                BorderPixels[_borderCount] = index;
                _borderCount++;
            }
            int PopBorder()
            {
                _borderCount--;
                return BorderPixels[_borderCount];
            }
        }
    }
}
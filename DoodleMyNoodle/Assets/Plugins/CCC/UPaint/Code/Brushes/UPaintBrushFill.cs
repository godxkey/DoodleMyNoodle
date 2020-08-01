using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;
using Unity.Collections;
using System;

namespace UPaintBrushes
{
    [System.Serializable]
    public class Fill : IUPaintBursh, IDisposable
    {
        public int ExtrusionCount;

        NativeArray<int> _pixelBuffer;
        DirtyValue<int> _pixelCount;

        public void OnPress(IUPaintBrushCanvasInterface canvas, in UPaintContext context)
        {
            // Allocate an array used in the job
            _pixelCount.Set(canvas.PreviewLayer.PixelCount);
            if (_pixelCount.ClearDirty())
            {
                if (_pixelBuffer.IsCreated)
                    _pixelBuffer.Dispose();
                _pixelBuffer = new NativeArray<int>(_pixelCount.Get(), Allocator.Persistent);
            }

            // find start position of the fill
            int2 pixelPosition = int2(round(context.CursorCoordinate));

            canvas.ScheduleNextBrushJob(new FillJob()
            {
                PixelBuffer = _pixelBuffer,
                StartCoordinates = pixelPosition,
                NewColor = context.Color,
                ReadLayer = canvas.MainLayer,
                WriteLayer = canvas.PreviewLayer,
                RemainingExtrusions = ExtrusionCount
            });

            canvas.ScheduleBlendPreviewOntoMainLayer();
        }

        public void OnHold(IUPaintBrushCanvasInterface canvas, in UPaintContext context)
        {
        }

        public void OnRelease(IUPaintBrushCanvasInterface canvas, in UPaintContext context)
        {
        }

        public void Dispose()
        {
            if (_pixelBuffer.IsCreated)
                _pixelBuffer.Dispose();
        }

        [BurstCompile]
        public struct FillJob : IJob
        {
            public NativeArray<int> PixelBuffer;
            public UPaintLayer WriteLayer;
            public int RemainingExtrusions;

            [ReadOnly] public UPaintLayer ReadLayer;
            [ReadOnly] public int2 StartCoordinates;
            [ReadOnly] public Color32 NewColor;

            private Color32 _replaceColor;
            private int _landCount;
            private int _borderCount;

            // utility method
            bool ColorEqual(in Color32 a, in Color32 b)
                => a.r == b.r
                && a.g == b.g
                && a.b == b.b
                && a.a == b.a;

            public void Execute()
            {
                // the color to replace is the color at the start of the fill
                _replaceColor = ReadLayer[StartCoordinates];
                if (ColorEqual(_replaceColor, NewColor))
                    return;

                if (WriteLayer.IsValidPixel(StartCoordinates))
                    PushAndPaintLand(StartCoordinates);


                ////////////////////////////////////////////////////////////////////////////////////////
                //      Fill
                ////////////////////////////////////////////////////////////////////////////////////////
                
                // while we have available land, extend it and paint it
                while (_landCount > 0)
                {
                    int2 pixelPosition = WriteLayer.GetPixelCoordinatesFromIndex(PopLand());

                    // check if there are new neighboring pixels to paint. Extend our land on those pixels
                    TestAndPaintPotentialLand(pixelPosition + int2(0, -1));// down
                    TestAndPaintPotentialLand(pixelPosition + int2(0, 1)); // up
                    TestAndPaintPotentialLand(pixelPosition + int2(-1, 0));// left
                    TestAndPaintPotentialLand(pixelPosition + int2(1, 0)); // right
                }
                RemainingExtrusions--;


                ////////////////////////////////////////////////////////////////////////////////////////
                //      Extrusion
                ////////////////////////////////////////////////////////////////////////////////////////
                while (RemainingExtrusions > 0)
                {
                    // convert all borders to lands
                    while (_borderCount > 0)
                    {
                        PushLand(PopBorder());
                    }

                    // for each land, extend and paint the border
                    while (_landCount > 0)
                    {
                        int land = PopLand();

                        int2 landPosition = WriteLayer.GetPixelCoordinatesFromIndex(land);

                        // check if there are new neighboring pixels to paint. Extend our border on those pixels
                        TestAndPaintPotentialBorder(landPosition + int2(0, -1));// down
                        TestAndPaintPotentialBorder(landPosition + int2(0, 1)); // up
                        TestAndPaintPotentialBorder(landPosition + int2(-1, 0));// left
                        TestAndPaintPotentialBorder(landPosition + int2(1, 0)); // right
                    }

                    RemainingExtrusions--;
                }
            }

            void PaintPixel(in int pixelIndex)
            {
                WriteLayer[pixelIndex] = NewColor;
            }

            void TestAndPaintPotentialLand(in int2 pixelPos)
            {
                if (WriteLayer.IsValidPixel(pixelPos) &&         // valid pixel ?
                    !ColorEqual(WriteLayer[pixelPos], NewColor))      // color not already filled there ?
                {
                    if (ColorEqual(ReadLayer[pixelPos], _replaceColor))   // corresponds to color we want to replace ?
                    {
                        PushAndPaintLand(pixelPos);
                    }
                    else if (RemainingExtrusions > 0)
                    {
                        PushAndPaintBorder(pixelPos);
                    }
                }
            }
            void TestAndPaintPotentialBorder(in int2 pixelPos)
            {
                if (WriteLayer.IsValidPixel(pixelPos) &&         // valid pixel ?
                    !ColorEqual(WriteLayer[pixelPos], NewColor))      // color not already filled there ?
                {
                    PushAndPaintBorder(pixelPos);
                }
            }

            void PushAndPaintLand(in int2 coordinates)
            {
                PushAndPaintLand(WriteLayer.GetPixelIndexFromCoordinates(coordinates));
            }
            void PushAndPaintLand(in int index)
            {
                PushLand(index);
                PaintPixel(index);
            }
            void PushLand(in int index)
            {
                PixelBuffer[_landCount] = index;
                _landCount++;
            }
            int PopLand()
            {
                _landCount--;
                return PixelBuffer[_landCount];
            }

            void PushAndPaintBorder(in int2 coordinates)
            {
                PushAndPaintBorder(WriteLayer.GetPixelIndexFromCoordinates(coordinates));
            }
            void PushAndPaintBorder(in int index)
            {
                PushBorder(index);
                PaintPixel(index);
            }
            void PushBorder(in int index)
            {
                PixelBuffer[PixelBuffer.Length - 1 - _borderCount] = index;
                _borderCount++;

                if (_borderCount + _landCount == WriteLayer.PixelCount)
                    throw new Exception();
            }
            int PopBorder()
            {
                _borderCount--;
                return PixelBuffer[PixelBuffer.Length - 1 - _borderCount];
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Mathematics;
using Unity.Burst;
using static Unity.Mathematics.math;

public static class UPaintOperations
{
    [BurstCompile]
    public static Color32 AlphaBlendColorOneOntoTwo(in Color32 one, in Color32 two)
    {
        // if both colors ar at alpha 0, return invalid color
        if (one.a == 0 && two.a == 0)
            return new Color32(0, 0, 0, 0);

        float4 col1 = new float4(one.r, one.g, one.b, one.a / 255f); // divide alphas by 255 to get a value between 0 and 1 (easier to work with)
        float4 col2 = new float4(two.r, two.g, two.b, two.a / 255f);

        // NB: Color blends while alpha adds up

        // ie. col1.a = 0.25   col2.a = 0.8
        // col1 contibutes: 0.25
        // col2 contibutes: (1 - 0.25) * 0.8   because col1 blocks 25% of the light
        // the blend between col1 and col2 is: 0.25 / (0.25 + 0.6) = 0.29
        // (0 being all col1 and 1 being all col2)

        float blendColor01 = col1.w / (col1.w + ((1 - col1.w) * col2.w));
        float3 resultCol = (col1.xyz * blendColor01) + (col2.xyz * (1 - blendColor01));

        // ie. col1.a = 0.25   col2.a = 0.8
        // col1 contributes: (1 - 0.8) * 0.25   (because col2 already blocks 80% of the light)
        // col1 contributes: 0.8
        // alpha adds up to: ((1 - 0.8) * 0.25) + 0.8
        //                   (   0.2  *   0.25) + 0.8
        //                           0.05       + 0.8 = 0.85
        float resultAlphaRatio = BlendAlphaOneOntoTwo(col1.w, col2.w);

        return new Color32(
            (byte)resultCol.x,
            (byte)resultCol.y,
            (byte)resultCol.z,
            (byte)(resultAlphaRatio * 255f));
    }

    /// <summary>
    /// NB: Alpha value must be between 0 and 255
    /// </summary>
    [BurstCompile]
    public static byte BlendAlphaOneOntoTwo(in byte a1, in byte a2)
    {
        return (byte)(BlendAlphaOneOntoTwo(a1 / 255f, a2 / 255f) * 255);
    }

    /// <summary>
    /// NB: Alpha value must be between 0 and 1
    /// </summary>
    [BurstCompile]
    public static float BlendAlphaOneOntoTwo(in float alphaRatio1, in float alphaRatio2)
    {
        // ie. a1 = 0.25   a2 = 0.8
        // col1 contributes: (1 - 0.8) * 0.25   (because col2 already blocks 80% of the light)
        // col1 contributes: 0.8
        // alpha adds up to: ((1 - 0.8) * 0.25) + 0.8
        //                   (   0.2  *   0.25) + 0.8
        //                           0.05       + 0.8 = 0.85
        return ((1 - alphaRatio2) * alphaRatio1) + alphaRatio2;
    }

    [BurstCompile]
    public static void ClampToLayerDimensions(in float2 min, in float2 max, in UPaintLayer layer, out int2 drawMin, out int2 drawMax)
    {
        drawMin = math.max(int2(min), int2(0,0));
        drawMax = math.min(int2(max), int2(layer.Width, layer.Height));
    }
}

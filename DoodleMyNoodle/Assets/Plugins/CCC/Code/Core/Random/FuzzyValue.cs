using Unity.Mathematics;

[System.Serializable]
public struct FuzzyValue
{
    /// <summary>
    /// The center of the random distribution.
    /// </summary>
    public float BaseValue;

    /// <summary>
    /// The width of the plateau in the random distribution, between 0 and 1. 0 will result in a triangular distribution /\. 1 will result in a flat distrubution —.
    /// </summary>
    public float DistributionPlateau;

    /// <summary>
    /// The maximum allowed variation from the base value. A variation of 0 will always produce 'BaseValue' as a result. 
    /// A variation of 20 will produce results inbetween [BaseValue - 20] and [BaseValue + 20]
    /// </summary>
    public float Variation;

    public float GetRandomValue(ref Random random)
    {
        return random.NextFloatPlateau(math.clamp(DistributionPlateau, 0, 1), BaseValue - Variation, BaseValue + Variation);
    }
}

public static class FuzzyValueRandomExtension
{
    /// <summary>
    /// Returns a random value between 0 and 1 with a plateau-shaped distribution. If you were to plot all the results, it would give a shape like this: /¯¯\
    /// </summary>
    /// <param name="distributionPlateau">The width of the plateau, between 0 and 1. 0 resulting in a triangular distribution /\. 1 resulting in a flat distrubution —.</param>
    public static float NextFloatPlateau(this ref Random random, float distributionPlateau)
    {
        if (distributionPlateau < 0 || distributionPlateau > 1)
            throw new System.ArgumentOutOfRangeException($"{nameof(distributionPlateau)} needs to be between 0 and 1");


        float t = 1 - distributionPlateau;
        float pAreaProportion = distributionPlateau / (distributionPlateau + (t * 0.5f));

        bool onPlateau = random.NextFloat() < pAreaProportion;

        float halfP = distributionPlateau / 2;
        if (onPlateau)
        {
            return random.NextFloat(0.5f - halfP, 0.5f + halfP);
        }
        else
        {
            float v1, v2;
            do
            {
                v1 = random.NextFloat();
                v2 = random.NextFloat();
            } while (v1 > v2);

            float halfT = t / 2;

            if (random.NextBool())
            {
                v1 = v1 * halfT + (0.5f + halfP);
            }
            else
            {
                v1 = v1 * -halfT + halfT;
            }

            return v1;
        }
    }

    /// <summary>
    /// Returns a random value between min and max with a plateau-shaped distribution. If you were to plot all the results, it would give a shape like this: /¯¯\
    /// </summary>
    /// <param name="distributionPlateau">The width of the plateau, between 0 and 1. 0 resulting in a triangular distribution /\. 1 resulting in a flat distrubution —.</param>
    public static float NextFloatPlateau(this ref Random random, float distributionPlateau, float min, float max)
    {
        return random.NextFloatPlateau(distributionPlateau) * (max - min) + min;
    }
}
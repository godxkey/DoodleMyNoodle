[System.Serializable]
public struct FixFuzzyValue
{
    /// <summary>
    /// The center of the random distribution.
    /// </summary>
    public fix BaseValue;

    /// <summary>
    /// The width of the plateau in the random distribution, between 0 and 1. 0 will result in a triangular distribution /\. 1 will result in a flat distrubution —.
    /// </summary>
    public fix DistributionPlateau;

    /// <summary>
    /// The maximum allowed variation from the base value. A variation of 0 will always produce 'BaseValue' as a result. 
    /// A variation of 20 will produce results inbetween [BaseValue - 20] and [BaseValue + 20]
    /// </summary>
    public fix Variation;

    public fix GetNext(ref FixRandom random)
    {
        return random.NextPlateau(BaseValue - Variation, BaseValue + Variation, DistributionPlateau);
    }

    public static explicit operator FixFuzzyValue(FuzzyValue fuzzyValue)
    {
        return new FixFuzzyValue()
        {
            BaseValue = (fix)fuzzyValue.BaseValue,
            DistributionPlateau = (fix)fuzzyValue.DistributionPlateau,
            Variation = (fix)fuzzyValue.Variation
        };
    }
}

public static class FixFuzzyValueRandomExtension
{
    /// <summary>
    /// Returns a random value between 0 and 1 with a plateau-shaped distribution. If you were to plot all the results, it would give a shape like this: /¯¯\
    /// </summary>
    /// <param name="distributionPlateau">The width of the plateau, between 0 and 1. 0 resulting in a triangular distribution /\. 1 resulting in a flat distrubution —.</param>
    public static fix NextPlateauRatio(this ref FixRandom random, fix distributionPlateau)
    {
        if (distributionPlateau < 0 || distributionPlateau > 1)
            throw new System.ArgumentOutOfRangeException($"{nameof(distributionPlateau)} needs to be between 0 and 1");


        fix t = 1 - distributionPlateau;
        fix pAreaProportion = distributionPlateau / (distributionPlateau + (t * fix.Half));

        bool onPlateau = random.NextFixRatio() < pAreaProportion;

        fix halfP = distributionPlateau / 2;
        if (onPlateau)
        {
            return random.NextFix(fix.Half - halfP, fix.Half + halfP);
        }
        else
        {
            fix v1, v2;
            do
            {
                v1 = random.NextFixRatio();
                v2 = random.NextFixRatio();
            } while (v1 > v2);

            fix halfT = t / 2;

            if (random.NextBool())
            {
                v1 = v1 * halfT + (fix.Half + halfP);
            }
            else
            {
                v1 = v1 * -halfT + halfT;
            }

            return v1;
        }
    }

    /// <summary>
    /// Returns a random value between 0 and 1 with a plateau-shaped distribution. If you were to plot all the results, it would give a shape like this: /¯¯\
    /// </summary>
    /// <param name="distributionPlateau">The width of the plateau, between 0 and 1. 0 resulting in a triangular distribution /\. 1 resulting in a flat distrubution —.</param>
    public static fix NextPlateau(this ref FixRandom random, fix min, fix max, fix distributionPlateau)
    {
        return NextPlateauRatio(ref random, distributionPlateau) * (max - min) + min;
    }
}
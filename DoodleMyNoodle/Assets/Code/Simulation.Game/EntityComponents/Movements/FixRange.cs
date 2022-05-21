public struct FixRange
{
    public fix Min;
    public fix Max;

    public static FixRange Infinite => new FixRange(fix.MinValue, fix.MaxValue);
    public static FixRange Invalid => new FixRange(0, -1);
    public FixRange(fix value)
    {
        Min = value;
        Max = value;
    }

    public FixRange(fix min, fix max)
    {
        Min = min;
        Max = max;
    }

    public bool IsValid => Max >= Min;
    public bool Contains(fix value) => value >= Min && value <= Max;
    public bool Contains(fix value, fix epsilon) => value >= Min - epsilon && value <= Max + epsilon;
    public fix Clamp(fix value) => fixMath.clamp(value, Min, Max);
}
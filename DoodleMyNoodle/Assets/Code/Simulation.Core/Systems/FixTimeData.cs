public readonly struct FixTimeData
{
    /// <summary>
    /// The total cumulative elapsed time in seconds.
    /// </summary>
    public readonly fix ElapsedTime;

    /// <summary>
    /// The time in seconds since the last time-updating event occurred. (For example, a frame.)
    /// </summary>
    public readonly fix DeltaTime;

    /// <summary>
    /// Create a new TimeData struct with the given values.
    /// </summary>
    /// <param name="elapsedTime">Time since the start of time collection.</param>
    /// <param name="deltaTime">Elapsed time since the last time-updating event occurred.</param>
    public FixTimeData(fix elapsedTime, fix deltaTime)
    {
        ElapsedTime = elapsedTime;
        DeltaTime = deltaTime;
    }
}

using UnityEngine;

/// <summary>
/// A timer with a duration that can expire.
/// </summary>
public struct SimCountdownTimer
{
    private Fix64 _expirationTime;

    public SimCountdownTimer(in Fix64 duration)
    {
        _expirationTime = Simulation.Time + duration;
    }

    public Fix64 RemainingTime
        => Simulation.Time - _expirationTime;

    public bool HasExpired
        => IsValid && Simulation.Time > _expirationTime;

    public void SetDuration(in Fix64 duration)
    {
        _expirationTime = Simulation.Time + duration;
    }

    public bool IsValid => _expirationTime != Fix64.Zero;
    public void Invalidate()
    {
        _expirationTime = Fix64.Zero;
    }
}
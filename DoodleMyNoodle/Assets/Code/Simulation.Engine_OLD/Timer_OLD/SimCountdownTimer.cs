using UnityEngine;

/// <summary>
/// A timer with a duration that can expire.
/// </summary>
public struct SimCountdownTimer
{
    private fix _expirationTime;

    public SimCountdownTimer(in fix duration)
    {
        _expirationTime = Simulation.Time + duration;
    }

    public fix RemainingTime
        => Simulation.Time - _expirationTime;

    public bool HasExpired
        => IsValid && Simulation.Time > _expirationTime;

    public void SetDuration(in fix duration)
    {
        _expirationTime = Simulation.Time + duration;
    }

    public bool IsValid => _expirationTime != fix.Zero;
    public void Invalidate()
    {
        _expirationTime = fix.Zero;
    }
}
using UnityEngine;

/// <summary>
/// A timer with a duration that can expire. NB: Should not be used in Simulation (use SimCountdownTimer for that)
/// </summary>
public struct CountdownTimer
{
    const float INVALID_EXPIRATION_TIME = 0;

    private float _expirationTime;

    public CountdownTimer(float duration)
    {
        _expirationTime = Time.time + duration;
    }

    public float RemainingTime
        => Time.time - _expirationTime;

    public bool HasExpired
        => IsValid && Time.time > _expirationTime;

    public void SetDuration(float duration)
    {
        _expirationTime = Time.time + duration;
    }

    public bool IsValid => _expirationTime != INVALID_EXPIRATION_TIME;
    public void Invalidate()
    {
        _expirationTime = INVALID_EXPIRATION_TIME;
    }
}
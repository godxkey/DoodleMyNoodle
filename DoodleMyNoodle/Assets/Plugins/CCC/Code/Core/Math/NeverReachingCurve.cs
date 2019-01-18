using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A curve that will tend towards its target, never reaching it
/// Similar to Lerp, but alpha can go from 'minX' to positive infinity
/// View online: https://www.desmos.com/calculator/botoga5bml
/// </summary>
//[System.Serializable]
public struct NeverReachingCurve
{
    public float A
    {
        get { return _a; }
        set
        {
            _a = value;
            Calculate_O_and_P();
        }
    }
    public float B
    {
        get { return _b; }
        set
        {
            _b = value;
            Calculate_O_and_P();
        }
    }
    public float Speed
    {
        get { return _speed; }
        set
        {
            _speed = value;
            Calculate_O_and_P();
        }
    }
    public float MinX
    {
        get { return _minX; }
        set
        {
            _minX = value;
            Calculate_P();
        }
    }
    private float _a;
    private float _b;
    private float _speed;
    private float _minX;

    private float _p;
    private float _o;

    private void Calculate_P()
    {
        _p = _minX + _o;
    }
    private void Calculate_O_and_P()
    {
        _o = 1 / (_speed * (_a - _b));
        Calculate_P();
    }

    public NeverReachingCurve(float a, float b, float speed, float minX = 0)
    {
        _a = a;
        _b = b;
        _speed = speed;
        _minX = minX;
        _o = 0;
        _p = 0;
        Calculate_O_and_P();
    }
    public float Evalutate(float x)
    {
        return (1 / (_speed * (-x + _p))) + _b;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is NeverReachingCurve))
            return false;

        NeverReachingCurve c = (NeverReachingCurve)obj;

        return _a == c.A
            && _b == c.B
            && _speed == c.Speed
            && _minX == c.MinX;
    }

    public override int GetHashCode()
    {
        return Mathf.RoundToInt(_a + _b + _speed + _minX);
    }

    public override string ToString()
    {
        return "NRC: a=" + A + "    b=" + B + "    speed=" + Speed + "    minX=" + MinX;
    }

    public static bool operator ==(NeverReachingCurve i, NeverReachingCurve j)
    {
        return i.Equals(j);
    }
    public static bool operator !=(NeverReachingCurve i, NeverReachingCurve j)
    {
        return !(i == j);
    }
}

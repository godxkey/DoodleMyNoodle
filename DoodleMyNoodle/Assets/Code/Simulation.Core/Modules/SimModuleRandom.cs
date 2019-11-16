using System;
using System.Collections.Generic;

internal class SimModuleRandom : SimModuleBase
{
    internal int RandomInt()
    {
        return GetGenerator().Next();
    }

    internal uint RandomUInt()
    {
        return GetGenerator().NextUInt();
    }

    internal bool RandomBool()
    {
        return GetGenerator().NextBool();
    }

    internal Fix64 Random01()
    {
        return GetGenerator().NextFix01();
    }

    internal Fix64 RandomRange(Fix64 min, Fix64 max)
    {
        return GetGenerator().RandomRange(min, max);
    }

    /// <summary>
    /// Vector will be normalized
    /// </summary>
    internal FixVector2 RandomDirection2D()
    {
        return GetGenerator().RandomDirection2D();
    }

    /// <summary>
    /// Vector will be normalized
    /// </summary>
    internal FixVector3 RandomDirection3D()
    {
        return GetGenerator().RandomDirection3D();
    }


    ////////////////////////////////////////////////////////////////////////////////////////
    //      This logic allows us to keep a deterministic RNG based on the current sim tick                                 
    ////////////////////////////////////////////////////////////////////////////////////////
    class TickRandom : FixRandom
    {
        public TickRandom() : base(0) { }

        public uint associatedTick
        {
            get { return _associatedTick; }
            set
            {
                if (_associatedTick != value)
                {
                    // ensures the seed is far apart from tick to tick
                    Reinitialise(SimModules._World.Seed + BitConverterX.UInt32ToInt32(value * 99877)); 
                }
                _associatedTick = value;
            }
        }

        private uint _associatedTick = 0;
    }

    TickRandom _randomNumberGenerator;

    TickRandom GetGenerator()
    {
        if (_randomNumberGenerator == null)
        {
            _randomNumberGenerator = new TickRandom();
        }

        _randomNumberGenerator.associatedTick = SimModules._Ticker.TickId;

        return _randomNumberGenerator;
    }
}
using System.Collections;
using System.Collections.Generic;

public struct WorldModuleTickRandom
{
    private FixRandom _randomFromTick;

    public WorldModuleTickRandom(uint tickId, uint seed)
    {
        _randomFromTick = new FixRandom(BitConverterX.UInt32ToInt32(((tickId + 1) * 99877) + seed));

        uint mod = tickId % 7;
        for (int i = 0; i < mod; i++)
        {
            _randomFromTick.NextUInt();
        }
    }

    /// <summary>
    /// We randomly pick a seed (based on the current tick, the current seed, and the amount of times 'PickRandomGenerator()' was called)
    /// </summary>
    public FixRandom PickRandomGenerator() => new FixRandom(_randomFromTick.NextInt());
}

using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class LevelMobWaveData
{
    public struct MobSpawn
    {
        public GameObject SimAsset;
        public MobSpawmModifierFlags MobModifierFlags;
        public Vector3 Position;
    }
    public List<MobSpawn> MobSpawns = new List<MobSpawn>();
}

using System.Collections.Generic;
using UnityEngine;

public class LevelData
{
    public struct MobSpawn
    {
        public GameObject SimAsset;
        public MobModifierFlags MobModifierFlags;
        public Vector3 Position;
    }
    public List<MobSpawn> MobSpawns = new List<MobSpawn>();
}

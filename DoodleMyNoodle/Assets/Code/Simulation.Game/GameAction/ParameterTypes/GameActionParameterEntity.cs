using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;

public class GameActionParameterEntity
{
    public class Description : GameAction.ParameterDescription
    {
        public int RangeFromInstigator { get; private set; }
        public bool IncludeSelf = true;
        public bool RequiresAttackableEntity = false;

        public Description() { }

        public override GameAction.ParameterDescriptionType GetParameterDescriptionType()
        {
            return GameAction.ParameterDescriptionType.Entity;
        }
    }

    [NetSerializable]
    public class Data : GameAction.ParameterData
    {
        public fix2 EntityPos;

        public Data() { }

        public Data(fix2 EntityPos)
        {
            this.EntityPos = EntityPos;
        }
    }
}
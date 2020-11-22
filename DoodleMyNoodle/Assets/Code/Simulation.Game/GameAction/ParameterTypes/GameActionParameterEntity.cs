using System;
using Unity.Entities;
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
        public Entity Entity;

        public Data() { }

        public Data(Entity Entity)
        {
            this.Entity = Entity;
        }
    }
}
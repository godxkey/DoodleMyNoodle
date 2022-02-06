using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;

public class GameActionParameterEntity
{
    public class Description : Action.ParameterDescription
    {
        public fix RangeFromInstigator = new fix(9999);
        public bool IncludeSelf = true;
        public bool RequiresAttackableEntity = false;
        public Func<ISimWorldReadAccessor, Entity, bool> CustomPredicate = null;

        public Description() { }

        public override Action.ParameterDescriptionType GetParameterDescriptionType()
        {
            return Action.ParameterDescriptionType.Entity;
        }
    }

    [NetSerializable]
    public class Data : Action.ParameterData
    {
        public Entity Entity;

        public Data() { }

        public Data(Entity entity)
        {
            this.Entity = entity;
        }

        public override string ToString()
        {
            return $"Entity({Entity})";
        }
    }
}
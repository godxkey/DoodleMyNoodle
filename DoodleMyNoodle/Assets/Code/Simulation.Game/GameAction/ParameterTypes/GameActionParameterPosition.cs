using System;
using UnityEngine;
using UnityEngineX;

public class GameActionParameterPosition
{
    public class Description : Action.ParameterDescription
    {
        public fix MaxRangeFromInstigator = fix.MaxValue;

        public override Action.ParameterDescriptionType GetParameterDescriptionType()
        {
            return Action.ParameterDescriptionType.Position;
        }
    }

    [NetSerializable]
    public class Data : Action.ParameterData
    {
        public fix2 Position;

        public Data() { }

        public Data(fix2 position)
        {
            Position = position;
        }

        public override string ToString()
        {
            return $"Position({Position})";
        }
    }
}
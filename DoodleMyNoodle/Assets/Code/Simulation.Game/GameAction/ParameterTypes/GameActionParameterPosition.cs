using System;
using UnityEngine;
using UnityEngineX;

public class GameActionParameterPosition
{
    public class Description : GameAction.ParameterDescription
    {
        public fix MaxRangeFromInstigator = fix.MaxValue;

        public override GameAction.ParameterDescriptionType GetParameterDescriptionType()
        {
            return GameAction.ParameterDescriptionType.Position;
        }
    }

    [NetSerializable]
    public class Data : GameAction.ParameterData
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
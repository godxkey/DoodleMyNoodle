using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

public class ConvertToSimEntity : ConvertToEntityMultiWorld
{
    public override GameWorldType WorldToConvertTo => GameWorldType.Simulation;

    protected override void Awake()
    {
        FillLocalFixTransformData();

        base.Awake();
    }

    public void FillLocalFixTransformData()
    {
        if (gameObject.HasComponent<NoTransform>())
            return;

        gameObject.GetOrAddComponent<FixTransformAuth>();
    }
}


using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(BlueprintDefinition))]
public class ConvertBlueprintToEntities : ConvertToEntity
{
    void Awake()
    {
        // let parent handle the conversion we have one
        Transform parent = transform.parent;
        if (parent && parent.GetComponent<ConvertBlueprintToEntities>())
            return;

        //PrepareChildForConversion(GameWorldType.Presentation, null);
        PrepareChildForConversion(GameWorldType.Simulation, null);
        Destroy(gameObject);
    }

    void PrepareChildForConversion(GameWorldType worldType, Transform newParent)
    {
        BlueprintDefinition blueprintDefinition = GetComponent<BlueprintDefinition>();
        GameObject childGO = blueprintDefinition.GetGameObject(worldType);

        if (!childGO)
            return;

        Transform childTr = childGO.transform;
        ConvertToEntityMultiWorld childConverter = childGO.GetComponent<ConvertToEntityMultiWorld>();

        // Make sure the child has a conversion component with the correct data
        if (childConverter)
        {
            if (childConverter.WorldToConvertTo != worldType)
            {
                Debug.LogError($"Child '{childGO.name}' of blueprint '{gameObject.name}' doesn't the expected conversion world.");
            }
        }
        else
        {
            switch (worldType)
            {
                case GameWorldType.Simulation:
                    childConverter = childGO.AddComponent<ConvertToSimEntity>();
                    break;
                case GameWorldType.Presentation:
                    childConverter = childGO.AddComponent<ConvertToViewEntity>();
                    break;
            }
            childConverter.ConversionMode = ConversionMode;
        }

        // add the blueprint Id
        childGO.AddComponent<BlueprintIdAuth>().Value = blueprintDefinition.BlueprintIdValue;

        // separate the child
        Transform tr = transform;
        childTr.SetParent(newParent, worldPositionStays: false);
        childTr.localPosition = tr.localPosition;
        childTr.localRotation = tr.localRotation;
        childTr.localScale = tr.localScale;
        if (childConverter is ConvertToSimEntity simConverter)
            simConverter.FillLocalFixTransformData();

        // if we have blueprints under our hierarchy ...
        int childCount = tr.childCount;
        for (int i = 0; i < childCount; i++)
        {
            // recursive 'PrepareChildForConversion'
            Transform child = tr.GetChild(i);
            if (child.TryGetComponent(out ConvertBlueprintToEntities subConvert))
            {
                subConvert.PrepareChildForConversion(worldType, childTr);
            }
        }
    }


}
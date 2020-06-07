using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(ViewBindingDefinition))]
public class ConvertViewBindingDefinitionToEntities : ConvertToEntity, IConvertGameObjectToEntity
{
    void Awake()
    {
        // let parent handle the conversion if we have one
        Transform parent = transform.parent;
        if (parent && parent.GetComponent<ConvertViewBindingDefinitionToEntities>())
            return;

        //PrepareChildForConversion(GameWorldType.Presentation, null);
        PrepareChildForConversion(GameWorldType.Simulation, null);
        Destroy(gameObject);
    }

    void PrepareChildForConversion(GameWorldType worldType, Transform newParent)
    {
        ViewBindingDefinition simToViewBinding = GetComponent<ViewBindingDefinition>();
        GameObject childGO = simToViewBinding.GetGameObject(worldType);

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
            if (child.TryGetComponent(out ConvertViewBindingDefinitionToEntities subConvert))
            {
                subConvert.PrepareChildForConversion(worldType, childTr);
            }
        }
    }


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (!gameObject.scene.IsValid())
        {
            Debug.LogWarning($"The prefab {gameObject.name} (a ViewBindingDefinition) is being converted to an entity. This should not happen." +
                $" You probably want to reference and convert the sim entity {GetComponent<ViewBindingDefinition>().GetSimGameObject().name} instead.");
            return;
        }
    }
}
using Unity.Entities;
using UnityEngine;
using System.Linq;
using System;
using CCC.InspectorDisplay;
using System.Collections.Generic;
using UnityEngineX.InspectorDisplay;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ItemAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    // SIMULATION

    // Game Action
    public string Value;

    [SerializeReference]
    [AlwaysExpand]
    public List<GameActionSettingAuthBase> GameActionSettings = new List<GameActionSettingAuthBase>();

    public bool HasCooldown = true;
    [AlwaysExpand]
    public ItemCooldownDataAuth CooldownAuth;
    public bool IsStackable = true;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, GameActionBank.GetActionId(Value));

        foreach (GameActionSettingAuthBase GameActionSetting in GameActionSettings)
        {
            GameActionSetting.Convert(entity, dstManager, conversionSystem);
        }

        if (Animation != null)
        {
            dstManager.AddComponentData(entity, new GameActionAnimationTypeData() { AnimationType = (int)Animation.AnimationType, Duration = (fix)Animation.Duration });
        }

        if (HasCooldown)
        {
            CooldownAuth.Convert(entity, dstManager, conversionSystem);
        }

        if (IsStackable)
        {
            dstManager.AddComponentData(entity, new ItemStackableData());
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        foreach (GameActionSettingAuthBase GameActionSetting in GameActionSettings)
        {
            GameActionSetting.DeclareReferencedPrefabs(referencedPrefabs);
        }
    }

    // PRESENTATION

    // Description
    public Sprite Icon;
    public string Name;
    public string EffectDescription;
    public AudioPlayable SfxOnUse;

    public bool PlayAnimation = false;
    public AnimationDefinition Animation;

    // Surveys
    public List<SurveyBaseController> CustomSurveys;

    public SurveyBaseController FindCustomSurveyPrefabForParameters(params GameAction.ParameterDescription[] parameters)
    {
        if (parameters.Length == 0)
        {
            return null;
        }

        // example: 3 parameters
        // try find survey for all 3 params
        // then try find survey for 2 params
        // then try find survey for 1 params
        // return null

        SurveyBaseController result = null;
        for (int i = parameters.Length; i > 0; i--)
        {
            result = TryFindCustomSurveyPrefabForParametersSubset(parameters, i);
            if (result != null)
                break;
        }

        return result;
    }

    private SurveyBaseController TryFindCustomSurveyPrefabForParametersSubset(GameAction.ParameterDescription[] parameters, int paramCount)
    {
        foreach (SurveyBaseController survey in CustomSurveys)
        {
            bool hasAllTypes = false;

            for (int i = 0; i < paramCount; i++)
            {
                if (Array.IndexOf(survey.ExpectedQuery, parameters[i].GetParameterDescriptionType()) != 0)
                {
                    hasAllTypes = true;
                }
                else
                {
                    hasAllTypes = false;
                    break;
                }
            }

            if (hasAllTypes)
            {
                return survey;
            }
        }

        return null;
    }
}
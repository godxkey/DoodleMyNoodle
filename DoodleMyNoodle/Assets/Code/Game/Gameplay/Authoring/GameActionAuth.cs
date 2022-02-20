using Unity.Entities;
using UnityEngine;
using System.Linq;
using System;
using CCC.InspectorDisplay;
using System.Collections.Generic;
using UnityEngineX.InspectorDisplay;
using UnityEngineX;
using System.Reflection;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
public class GameActionAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    // SIMULATION

    // Game Action
    public string Value;

    [SerializeReference]
    [AlwaysExpand]
    public List<GameActionSettingAuthBase> GameActionSettings = new List<GameActionSettingAuthBase>();

    private bool _hasUpdatedListOfGameActionSettings = false;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, GameActionBank.GetActionId(Value));

        // Update list of GameActionSettingAuth by adding any missing instances and removing any extra
        UpdateGameActionSettingsList();

        // Convert all GameActionSettingAuths
        foreach (GameActionSettingAuthBase setting in GameActionSettings)
        {
            setting.Context = gameObject;
            setting.Convert(entity, dstManager, conversionSystem);
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        UpdateGameActionSettingsList();

        foreach (GameActionSettingAuthBase settings in GameActionSettings)
        {
            settings.DeclareReferencedPrefabs(referencedPrefabs);
        }
    }

    private void UpdateGameActionSettingsList()
    {
        if (_hasUpdatedListOfGameActionSettings)
            return;

        var gameAction = GameActionBank.GetAction(Value);
        var requiredSettingAuths = GameActionSettingAuthBase.GetRequiredSettingAuthTypes(gameAction.GetType());
        GameActionSettings.RemoveAll(authInstance => authInstance == null || !requiredSettingAuths.Contains(authInstance.GetType()));
        foreach (var authType in requiredSettingAuths)
        {
            if (!GameActionSettings.Any(x => x.GetType() == authType))
            {
                GameActionSettings.Add(Activator.CreateInstance(authType) as GameActionSettingAuthBase);
            }
        }
        _hasUpdatedListOfGameActionSettings = true;
    }

    // PRESENTATION

    // Description
    public AudioPlayable SfxOnUse;
    public AnimationDefinition Animation;

    public VFXDefinition InstigatorVFX;
    public VFXDefinition TargetsVFX;

    // Surveys
    public List<SurveyBaseController> Surveys;

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
        foreach (SurveyBaseController survey in Surveys)
        {
            bool hasAllTypes = false;

            for (int i = 0; i < paramCount; i++)
            {
                if (Array.IndexOf(survey.ExpectedQuery, parameters[i].GetParameterDescriptionType()) != -1)
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
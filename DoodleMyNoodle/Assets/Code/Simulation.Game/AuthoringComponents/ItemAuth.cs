﻿using CCC.InspectorDisplay;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ItemAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    [System.Serializable]
    public struct SurveyReference
    {
        public List<GameAction.ParameterDescriptionType> ParameterTypes;
        public GameObject Survey;
    }

    public ItemVisualInfo ItemVisualInfo;

    public bool UseSpecificSurveys = false;
    [ShowIf("UseSpecificSurveys")]
    public List<SurveyReference> SurveyList;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        
    }

    public GameObject FindSurveyDefinitionForParameters(params GameAction.ParameterDescription[] parameters)
    {
        if (parameters.Length == 0 || UseSpecificSurveys == false)
        {
            Debug.LogWarning("Can't find a survey since parameters are empty");
            return null;
        }

        // TODO : Handle case where we multiple survey with multiple parameters
        foreach (SurveyReference survey in SurveyList)
        {
            bool hasAllTypes = false;
            foreach (GameAction.ParameterDescription parameter in parameters)
            {
                if (survey.ParameterTypes.Contains(parameter.GetParameterDescriptionType()))
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
                return survey.Survey;
            }
        }

        return null;
    }
}
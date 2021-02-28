using CCC.InspectorDisplay;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ItemAuth : MonoBehaviour
{
    [System.Serializable]
    public struct SurveyReference
    {
        public List<GameAction.ParameterDescriptionType> ParameterTypes;
        public GameObject Survey;
    }

    public ItemVisualInfo ItemVisualInfo;
    public AudioPlayable SfxOnUse;

    public bool UseSpecificSurveys = false;
    [ShowIf("UseSpecificSurveys")]
    public List<SurveyReference> SurveyList;

    public GameObject FindSurveyDefinitionForParameters(params GameAction.ParameterDescription[] parameters)
    {
        if (parameters.Length == 0 || UseSpecificSurveys == false)
        {
            // I commented this since it's a very normal scenario. We don't need to be warned in the editor
            // Debug.LogWarning("Can't find a survey since parameters are empty");
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
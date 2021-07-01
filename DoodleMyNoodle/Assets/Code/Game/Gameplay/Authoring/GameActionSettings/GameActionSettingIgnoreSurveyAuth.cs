using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using System;
using UnityEngine.Scripting.APIUpdating;

[Serializable]
[GameActionSettingAuth(typeof(GameActionSettingIgnoreSurvey))]
[MovedFrom(false, sourceClassName: "GameActionSettingIgnoreSurveyAuth")]
public class GameActionSettingIgnoreSurveyAuth : GameActionSettingAuthBase
{
    public bool IgnoreSurvey;

    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GameActionSettingIgnoreSurvey()
        {
            IgnoreSurvey = IgnoreSurvey
        });
    }
}
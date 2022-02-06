using Unity.Mathematics;
using static Unity.Mathematics.math;
using System;
using UnityEngine;

public class GameActionScriptTemplate : ScriptTemplate
{
    public override string GetScriptContent()
    {
        // When refering to a type, it is recommended to use {typeof(TheType).GetPrettyName()}.
        // This helps keeping the templates up-to-date when types are renamed or removed.

        return
$@"using static fixMath;
using Unity.Entities;
using Unity.Collections;
using System;

public class #SCRIPTNAME# : {nameof(Action)}<#SCRIPTNAME#.Settings>
{{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {{
        public fix Range;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {{
            dstManager.AddComponentData(entity, new Settings()
            {{
            }});
        }}
    }}

    public struct Settings : IComponentData
    {{
        public fix Range;
    }}

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context, Settings settings)
    {{
        return new UseContract(
                   new GameActionParameterPosition.Description()
                   {{
                       MaxRangeFromInstigator = settings.Range
                   }});
    }}

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, ref ResultData resultData, Settings settings)
    {{
        if (parameters.TryGetParameter(0, out GameActionParameterPosition.Data paramPosition))
        {{
            return true;
        }}

        return false;
    }}
}}";
    }

    public override string GetScriptDefaultName()
    {
        return "NewGameAction";
    }
}

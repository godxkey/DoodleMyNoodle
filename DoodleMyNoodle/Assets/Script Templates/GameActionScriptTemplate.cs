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
using System;
using System.Collections.Generic;

public class #SCRIPTNAME# : {nameof(GameAction)}<#SCRIPTNAME#.Settings>
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
                Range = Range,
            }});
        }}
    }}

    public struct Settings : IComponentData
    {{
        public fix Range;
    }}

    protected override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, ref Settings settings)
    {{
        return new ExecutionContract();
    }}

    protected override bool Execute(in ExecInputs input, ref ExecOutput output, ref Settings settings)
    {{
        return false;
    }}
}}";
    }

    public override string GetScriptDefaultName()
    {
        return "NewGameAction";
    }
}

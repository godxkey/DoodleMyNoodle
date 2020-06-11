using Unity.Mathematics;
using static Unity.Mathematics.math;
using System;
using UnityEngine;


public class GameplayScriptTemplate : ScriptTemplate
{
    public override string GetScriptContent()
    {
        // When refering to a type, it is recommended to use {typeof(TheType).GetPrettyName()}.
        // This helps keeping the templates up-to-date when types are renamed or removed.

        return
$@"using System;
using UnityEngine;
using {nameof(UnityEngineX)};

public class #SCRIPTNAME# : {nameof(GameMonoBehaviour)}
{{
    public override void OnGameStart()
    {{

    }}
    
    public override void OnGameUpdate()
    {{

    }}
}}";
    }

    public override string GetScriptDefaultName()
    {
        return "NewGameplayScript";
    }
}

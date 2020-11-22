using System;
using UnityEngine;
using UnityEngineX;


public class CustomMiniGameScriptTemplate : ScriptTemplate
{
    public override string GetScriptContent()
    {
        // When refering to a type, it is recommended to use {typeof(TheType).GetPrettyName()}.
        // This helps keeping the templates up-to-date when types are renamed or removed.

        return
$@"using System.Collections;
using System.Collections.Generic;

public class #SCRIPTNAME# : {nameof(SurveyBaseController)}
{{

    protected override List<GameAction.ParameterData> GetResult()
    {{
        List<GameAction.ParameterData> results = new List<GameAction.ParameterData>();
        return results;
    }}

    protected override string GetDebugResult()
    {{
        return ""Result : "";
    }}

    protected override void OnUpdate()
    {{
        
    }}

    protected override IEnumerator SurveyLoop()
    {{
        while (true)
        {{
            yield return null;
        }}
    }}
}}
";
    }

    public override string GetScriptDefaultName()
    {
        return "NewScriptTemplate";
    }
}
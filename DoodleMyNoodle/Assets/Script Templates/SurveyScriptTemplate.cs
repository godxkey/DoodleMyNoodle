using System;
using UnityEngine;
using UnityEngineX;


public class SurveyScriptTemplate : ScriptTemplate
{
    public override string GetScriptContent()
    {
        // When refering to a type, it is recommended to use {typeof(TheType).GetPrettyName()}.
        // This helps keeping the templates up-to-date when types are renamed or removed.

        return
$@"using System;
using System.Collections;
using System.Collections.Generic;

public class #SCRIPTNAME# : {nameof(SurveyBaseController2)}
{{
    protected override {nameof(GameAction)}.{nameof(GameAction.ParameterDescriptionType)}[] GetExpectedQuery() => new {nameof(GameAction)}.{nameof(GameAction.ParameterDescriptionType)}[]
    {{
        // list your expected query param types here
    }};

    protected override IEnumerator SurveyRoutine({nameof(GameAction)}.{nameof(GameAction.ParameterDescription)}[] queryParams, List<{nameof(GameAction)}.{nameof(GameAction.ParameterData)}> result, Action complete, Action cancel)
    {{
        complete();
        yield break;
    }}

    // Clean up
    protected override void OnEndSurvey(bool wasCompleted)
    {{
    }}
}}
";
    }

    public override string GetScriptDefaultName()
    {
        return "SurveyX";
    }
}
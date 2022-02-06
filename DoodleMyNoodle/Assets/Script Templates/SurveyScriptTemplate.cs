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

public class #SCRIPTNAME# : {nameof(SurveyBaseController)}
{{
    protected override {nameof(Action)}.{nameof(Action.ParameterDescriptionType)}[] GetExpectedQuery() => new {nameof(Action)}.{nameof(Action.ParameterDescriptionType)}[]
    {{
        // list your expected query param types here
    }};

    protected override IEnumerator SurveyRoutine({nameof(SurveyBaseController.Context)} context, List<{nameof(Action)}.{nameof(Action.ParameterData)}> result, Action complete, Action cancel)
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
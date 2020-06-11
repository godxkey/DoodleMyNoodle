using System;
using UnityEngine;

public class ScriptTemplateTemplate : ScriptTemplate
{
    public override string GetScriptContent()
    {
        return
$@"using System;
using UnityEngine;
using UnityEngineX;


public class #SCRIPTNAME# : ScriptTemplate
{{
    public override string GetScriptContent()
    {{
        // When refering to a type, it is recommended to use {{typeof(TheType).GetPrettyName()}}.
        // This helps keeping the templates up-to-date when types are renamed or removed.

        return
$@""using UnityEngine;

public class #%STRIPME%SCRIPTNAME%STRIPME%# : {{typeof({nameof(MonoBehaviour)}).GetPrettyName()}}
{{{{
    void Start()
    {{{{

    }}}}
    
    void Update()
    {{{{

    }}}}
}}}}"";
    }}

    public override string GetScriptDefaultName()
    {{
        return ""NewScriptTemplate"";
    }}
}}";
    }

    public override string GetScriptDefaultName()
    {
        return "NewScriptTemplate";
    }
}

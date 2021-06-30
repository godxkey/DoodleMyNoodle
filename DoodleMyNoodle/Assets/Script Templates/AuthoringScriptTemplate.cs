using System;
using UnityEngine;
using UnityEngineX;


public class AuthoringScriptTemplate : ScriptTemplate
{
    public override string GetScriptContent()
    {
        // When refering to a type, it is recommended to use {typeof(TheType).GetPrettyName()}.
        // This helps keeping the templates up-to-date when types are renamed or removed.

        return
$@"using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class #SCRIPTNAME# : MonoBehaviour, IConvertGameObjectToEntity
{{
    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {{
    }}
}}";
    }

    public override string GetScriptDefaultName()
    {
        return "XAuth";
    }
}
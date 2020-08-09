using System;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;


public class SimulationAuthoringScriptTemplate : ScriptTemplate
{
    public override string GetScriptContent()
    {
        // When refering to a type, it is recommended to use {typeof(TheType).GetPrettyName()}.
        // This helps keeping the templates up-to-date when types are renamed or removed.

        return
$@"using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class #SCRIPTNAME# : MonoBehaviour, {typeof(IConvertGameObjectToEntity).GetPrettyName()}
{{

    public void Convert({typeof(Entity).GetPrettyName()} entity, {typeof(EntityManager).GetPrettyName()} dstManager, {typeof(GameObjectConversionSystem).GetPrettyName()} conversionSystem)
    {{
    }}
}}";
    }

    public override string GetScriptDefaultName()
    {
        return "New Authoring Script";
    }
}
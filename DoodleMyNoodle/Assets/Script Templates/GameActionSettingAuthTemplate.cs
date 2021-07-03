using System;
using UnityEngine;
using UnityEngineX;


public class GameActionSettingAuthTemplate : ScriptTemplate
{
    public override string GetScriptContent()
    {
        // When refering to a type, it is recommended to use {typeof(TheType).GetPrettyName()}.
        // This helps keeping the templates up-to-date when types are renamed or removed.

        return
$@"using UnityEngine;
using System;
using Unity.Entities;

[Serializable]
[{typeof(GameActionSettingAuthAttribute).GetPrettyName()}(typeof(GameActionSettingX))]
public class #SCRIPTNAME# : {typeof(GameActionSettingAuthBase).GetPrettyName()}, IItemSettingDescription
{{
    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {{
        dstManager.AddComponentData(entity, new GameActionSettingX() {{ }});
    }}

    public Color GetColor()
    {{
        return Color.white;
    }}

    public string GetDescription()
    {{
        return $""Description d'item"";
    }}
}}";
    }

    public override string GetScriptDefaultName()
    {
        return "GameActionSettingXAuth";
    }
}
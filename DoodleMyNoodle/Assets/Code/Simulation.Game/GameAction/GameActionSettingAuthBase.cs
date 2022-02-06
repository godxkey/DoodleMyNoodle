using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Entities;
using System.Linq;
using UnityEngineX;
using System.Reflection;

public class GameActionSettingAuthAttribute : Attribute
{
    public Type SettingType;

    public GameActionSettingAuthAttribute(Type settingType) { SettingType = settingType; }
}

[Serializable]
public abstract class GameActionSettingAuthBase : IDeclareReferencedPrefabs
{
    [NonSerialized] // set manually, just before conversion
    public GameObject Context;

    // mandatory
    public abstract void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem);

    // optional
    public virtual void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs) { }



    private static Dictionary<Type, Type> s_sim2AuthType;
    private static Dictionary<Type, Type[]> s_gameAction2AuthTypes = new Dictionary<Type, Type[]>();

    public static Type[] GetRequiredSettingAuthTypes(Type gameActionType)
    {
        if (!s_gameAction2AuthTypes.TryGetValue(gameActionType, out Type[] result))
        {
            GameAction gameAction = (GameAction)Activator.CreateInstance(gameActionType);
            var allSettingAuths = GetGameActionSettingAuthTypes();

            result = gameAction.GetRequiredSettingTypes().Select((simType) =>
            {
                if (!allSettingAuths.TryGetValue(simType, out Type authType))
                {
                    Log.Error($"Game Action Setting Auth Type doesn't exist for {simType}");
                    return null;
                }
                else
                {
                    return authType;
                }
            }).ToArray();
            s_gameAction2AuthTypes.Add(gameActionType, result);
        }

        return result;
    }

    private static Dictionary<Type, Type> GetGameActionSettingAuthTypes()
    {
        if (s_sim2AuthType == null)
        {
            s_sim2AuthType = new Dictionary<Type, Type>();

            Type[] gameActionSettingAuthTypes = TypeUtility.GetTypesWithAttribute(typeof(GameActionSettingAuthAttribute)).ToArray();
            foreach (Type gameActionSettingAuthType in gameActionSettingAuthTypes)
            {
                GameActionSettingAuthAttribute attribute = (GameActionSettingAuthAttribute)gameActionSettingAuthType.GetCustomAttribute(typeof(GameActionSettingAuthAttribute));
                if (!s_sim2AuthType.ContainsKey(attribute.SettingType))
                {
                    s_sim2AuthType.Add(attribute.SettingType, gameActionSettingAuthType);
                }
            }
        }
        return s_sim2AuthType;
    }
}

//public class FieldValueBase { }

//public class FieldValueGameObject : FieldValueBase
//{
//    public GameObject Value;
//}

//public class FieldValueInt : FieldValueBase
//{
//    public int Value;
//}

//public class FieldValueFix : FieldValueBase
//{
//    public fix Value;
//}

//public class Field
//{
//    public string Name;

//    [SerializeReference]
//    public FieldValueBase Value;
//}

//[Serializable]
//public class GameActionSettingAuthAutoMagic : GameActionSettingAuthBase
//{
//    public List<Field> Fields = new List<Field>();

//    [NonSerialized] // set manually, just before conversion
//    public Type ContextSimType;

//    // mandatory
//    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
//    {
//        var simComponentType = ContextSimType;
//        var simComponent = Activator.CreateInstance(simComponentType);

//        MethodInfo method = typeof(EntityManager).GetMethod(nameof(EntityManager.GetComponentData));
//        MethodInfo generic = method.MakeGenericMethod(simComponentType);
//        generic.Invoke(dstManager, new object[] { entity, simComponent});
//    }

//    // optional
//    public override void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs) { }
//}
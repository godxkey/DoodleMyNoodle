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
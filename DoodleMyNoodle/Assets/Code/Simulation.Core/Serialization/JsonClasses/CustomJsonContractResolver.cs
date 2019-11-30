using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

class CustomJsonContractResolver : DefaultContractResolver
{
    protected override List<MemberInfo> GetSerializableMembers(Type objectType)
    {
        // exclude all properties!
        FieldInfo[] fieldInfos = objectType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        List<MemberInfo> serializedMembers = new List<MemberInfo>(fieldInfos.Length);// = base.GetSerializableMembers(objectType);
        for (int i = 0; i < fieldInfos.Length; i++)
        {
            if (!fieldInfos[i].CustomAttributes.Contains<System.NonSerializedAttribute>())
            {
                serializedMembers.Add(fieldInfos[i]);
            }
        }

        return serializedMembers; //.Where(m => m.MemberType == MemberTypes.Field).ToList();
    }

    protected override JsonContract CreateContract(Type objectType)
    {
        // if the type is a dictionary, serialize it as a list of key-value pairs
        if (objectType.GetInterfaces().Any(i => i == typeof(IDictionary) ||
           (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>))))
        {
            return base.CreateArrayContract(objectType);
        }


        // do not serialize Value types as references
        JsonContract contract = base.CreateContract(objectType);
        if (objectType.IsValueType)
        {
            contract.IsReference = false;
        }

        return contract;
    }
}
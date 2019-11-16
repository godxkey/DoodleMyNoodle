using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

class CustomJsonContractResolver : DefaultContractResolver
{
    protected override List<MemberInfo> GetSerializableMembers(Type objectType)
    {
        // exclude all properties!
        List<MemberInfo> serializedMembers = base.GetSerializableMembers(objectType);
        return serializedMembers.Where(m => m.MemberType == MemberTypes.Field).ToList();
    }

    protected override JsonContract CreateContract(Type objectType)
    {
        JsonContract contract = base.CreateContract(objectType);

        if (objectType.IsValueType)
        {
            contract.IsReference = false;
        }

        return contract;
    }
}
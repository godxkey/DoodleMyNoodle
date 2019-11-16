using Newtonsoft.Json;
using System;
using UnityEngine;

public class IDTypeJsonConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        IDType idType = (IDType)value;
        writer.WriteValue(idType.GetValue());
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if(existingValue == null)
        {
            DebugService.LogError($"Error while deserializing id type {objectType}. There is no existing value. Is it a struct like it should ?");
            return null;
        }

        IDType idType = (IDType)existingValue;

        Int64 w = 2;
        UInt32 x = (UInt32)w;

        Int64 test = (Int64)reader.Value;

        idType.SetValue(test);

        return idType;
    }

    public override bool CanRead => true;

    public override bool CanConvert(Type objectType)
    {
        return typeof(IDType).IsAssignableFrom(objectType);
    }
}
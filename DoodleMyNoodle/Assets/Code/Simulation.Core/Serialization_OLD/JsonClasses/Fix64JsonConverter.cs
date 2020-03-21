using Newtonsoft.Json;
using System;
using UnityEngine;

public class Fix64JsonConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        Fix64 fix64 = (Fix64)value;
        writer.WriteValue(fix64.RawValue);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return Fix64.FromRaw(Convert.ToInt64(reader.Value));
    }

    public override bool CanRead => true;

    public override bool CanConvert(Type objectType)
    {
        return typeof(Fix64) == objectType;
    }
}
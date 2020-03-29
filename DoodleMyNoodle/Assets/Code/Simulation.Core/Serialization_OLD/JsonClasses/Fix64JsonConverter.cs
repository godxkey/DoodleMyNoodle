using Newtonsoft.Json;
using System;
using UnityEngine;

public class Fix64JsonConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        fix fix64 = (fix)value;
        writer.WriteValue(fix64.RawValue);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return fix.FromRaw(Convert.ToInt64(reader.Value));
    }

    public override bool CanRead => true;

    public override bool CanConvert(Type objectType)
    {
        return typeof(fix) == objectType;
    }
}
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
        IDType idObject = null;

        if(existingValue != null)
        {
            // get the existing object 
            idObject = (IDType)existingValue;
        }
        else
        {
            // make an instance
            idObject = (IDType)Activator.CreateInstance(objectType);
        }


        // fill it up
        idObject.SetValue(reader.Value);

        return idObject;
    }

    public override bool CanRead => true;

    public override bool CanConvert(Type objectType)
    {
        return typeof(IDType).IsAssignableFrom(objectType);
    }
}
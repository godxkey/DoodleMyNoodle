using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class SimObjectJsonConverter : JsonConverter
{
    // serialization
    public SimBlueprintIdIndexMap BlueprintIdIndexMap;

    // deserialization
    public Dictionary<SimObjectId, SimObject> SimObjectsReferenceTable;
    public SimBlueprint[] AvailableBlueprints;

    List<SimObject> _cachedComponentList = new List<SimObject>();

    struct ReferenceData
    {
        public bool IsBlueprint;
        public uint Value1;
        public ushort Value2;

        public ulong ToUInt64()
        {
            ulong result = IsBlueprint ? 1u : 0u;
            result <<= 32;
            result |= Value1;
            result <<= 16;
            result |= Value2;
            return result;
        }

        public static ReferenceData FromUInt64(ulong data)
        {
            ReferenceData result = new ReferenceData();
            result.Value2 = (ushort)(data & ushort.MaxValue);
            data >>= 16;
            result.Value1 = (uint)(data & uint.MaxValue);
            data >>= 32;
            result.IsBlueprint = ((data & 1u) == 1u) ? true : false;
            return result;
        }
    }

    public override void WriteJson(JsonWriter writer, object obj, JsonSerializer serializer)
    {
        if (obj is SimObject simObj)
        {
            ReferenceData refData = new ReferenceData() { Value1 = uint.MaxValue, Value2 = ushort.MaxValue };

            // this means we're referencing an entity that was never added to the sim runtime
            // We're referencing a blueprint (like a prefab)
            if (simObj.SimObjectId.IsValid == false)
            {
                refData.IsBlueprint = true;

                SimEntity entity = simObj.GetComponent<SimEntity>();

                if (entity)
                {
                    // write blueprint Id index
                    refData.Value1 = BlueprintIdIndexMap.GetIndexFromBlueprintId(entity.BlueprintId);

                    entity.GetComponents(_cachedComponentList);

                    // write component index
                    refData.Value2 = (ushort)_cachedComponentList.IndexOf(simObj);
                }
                else
                {
                    Debug.LogError("Someone is referencing a SimObject that has no SimEntity component");
                }

            }
            else
            {
                refData.IsBlueprint = false;

                // write simObjectId
                refData.Value1 = simObj.SimObjectId.Value;
            }

            writer.WriteValue(refData.ToUInt64());
        }
        else
        {
            serializer.Serialize(writer, obj, obj.GetType());
        }
    }

    bool _skipNextConvert;

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        object val = reader.Value;

        // If the serialized object is not a SimObjectId, it will manifest with a "StartObject" token
        //  In that case, we'll want to deserialize using json's default deserializer
        if (reader.TokenType == JsonToken.StartObject)
        {
            _skipNextConvert = true;
            return serializer.Deserialize(reader, objectType);
        }

        if (val == null) // the SimObject reference was null
        {
            return null;
        }

        // Json misinterprets the values to 'long' and 'int'
        if (val is long longValue)
        {
            val = (ulong)longValue;
        }
        else if (val is int intValue)
        {
            val = (ulong)intValue;
        }

        if (val is ulong readValue)
        {
            ReferenceData refData = ReferenceData.FromUInt64(readValue);

            if (refData.IsBlueprint)
            {
                uint blueprintIndex = refData.Value1;
                if (blueprintIndex < AvailableBlueprints.Length)
                {
                    SimBlueprint blueprint = AvailableBlueprints[blueprintIndex];

                    blueprint.Prefab.GetComponents(_cachedComponentList);

                    int componentIndex = refData.Value2;
                    if (componentIndex < _cachedComponentList.Count)
                    {
                        DebugService.Log($"Reading ref: {reader.Path} : BLUEPRINT {_cachedComponentList[componentIndex].gameObject}'s {_cachedComponentList[componentIndex].GetType()}");
                        return _cachedComponentList[componentIndex];
                    }
                }
            }
            else
            {
                SimObjectId simObjId = new SimObjectId(refData.Value1);

                if (SimObjectsReferenceTable.TryGetValue(simObjId, out SimObject simObject))
                {
                    DebugService.Log($"Reading ref: {reader.Path} : OBJECT {simObject.gameObject.name}'s {simObject.GetType()}");
                    return simObject;
                }
            }

        }

        DebugService.Log($"Reading ref: {reader.Path} : FAILED {val}");
        return null;
    }

    public override bool CanRead => true;

    public override bool CanConvert(Type objectType)
    {
        if (_skipNextConvert)
        {
            _skipNextConvert = false;
            return false;
        }
        return typeof(SimObject).IsAssignableFrom(objectType) || objectType.IsInterface;
    }
}
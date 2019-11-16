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

    List<SimObject> _componentList = new List<SimObject>();

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

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        SimObject obj = (SimObject)value;

        ReferenceData refData = new ReferenceData() { Value1 = uint.MaxValue, Value2 = ushort.MaxValue };

        // this means we're referencing an entity that was never added to the sim runtime
        // We're referencing a blueprint (like a prefab)
        if (obj.SimObjectId.IsValid == false)
        {
            refData.IsBlueprint = true;

            SimEntity entity = obj.GetComponent<SimEntity>();

            if (entity)
            {
                // write blueprint Id index
                refData.Value1 = BlueprintIdIndexMap.GetIndexFromBlueprintId(entity.BlueprintId);

                entity.GetComponents(_componentList);

                // write component index
                refData.Value2 = (ushort)_componentList.IndexOf(obj);
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
            refData.Value1 = obj.SimObjectId.Value;
        }

        writer.WriteValue(refData.ToUInt64());
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        object val = reader.Value;
        if (val is ulong readValue)
        {
            ReferenceData refData = ReferenceData.FromUInt64(readValue);

            if (refData.IsBlueprint)
            {
                uint blueprintIndex = refData.Value1;
                if(blueprintIndex < AvailableBlueprints.Length)
                {
                    SimBlueprint blueprint = AvailableBlueprints[blueprintIndex];

                    blueprint.Prefab.GetComponents(_componentList);

                    int componentIndex = refData.Value2;
                    if(componentIndex < _componentList.Count)
                    {
                        return _componentList[componentIndex];
                    }
                }
            }
            else
            {
                SimObjectId simObjId = new SimObjectId(refData.Value1);

                if (SimObjectsReferenceTable.TryGetValue(simObjId, out SimObject simObject))
                {
                    return simObject;
                }
            }
        }

        DebugService.LogError($"Error in deserialization: Failed to recreate reference to simObject {val}.");
        return null;
    }

    public override bool CanRead => true;

    public override bool CanConvert(Type objectType)
    {
        return typeof(SimObject).IsAssignableFrom(objectType);
    }
}
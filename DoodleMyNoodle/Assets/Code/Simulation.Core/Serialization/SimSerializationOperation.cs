﻿using CCC.Operations;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Sim.Operations
{
    public class SimSerializationOperation : CoroutineOperation
    {
        [ConfigVar(name: "log.sim_serialization", defaultValue: "0", description: "Should the SimModuleSerializer print the serialization text after completing a serializing?", flags: ConfigVarFlag.Save)]
        static ConfigVar s_printSerializeResult;

        public bool PartialSuccess;
        public string SerializationData;

        SimObjectJsonConverter _simObjectJsonConverter;
        JsonSerializerSettings _jsonSerializerSettings;

        internal SimSerializationOperation(SimObjectJsonConverter simObjectJsonConverter, JsonSerializerSettings jsonSerializerSettings)
        {
            _simObjectJsonConverter = simObjectJsonConverter;
            _jsonSerializerSettings = jsonSerializerSettings;
        }

        protected override IEnumerator ExecuteRoutine()
        {
            // utility variables
            List<SimComponent> simComponentList = new List<SimComponent>();
            SimComponentDataStack componentDataStack = new SimComponentDataStack();
            SimBlueprintIdIndexMap blueprintIdIndexMap = new SimBlueprintIdIndexMap();
            _simObjectJsonConverter.BlueprintIdIndexMap = blueprintIdIndexMap; // necessary for json serialization
            SimWorld world = SimModules._World;


            DebugService.Log("Start Serialization Process...");

            ////////////////////////////////////////////////////////////////////////////////////////
            //      Fill data in serializedWorld                                 
            ////////////////////////////////////////////////////////////////////////////////////////
            SimSerializableWorld serializableWorld = new SimSerializableWorld();

            serializableWorld.TickId = world.TickId;
            serializableWorld.NextObjectId = world.NextObjectId;
            serializableWorld.Seed = world.Seed;

            serializableWorld.ObjectsThatHaventStartedYet = new List<SimObjectId>(world.ObjectsThatHaventStartedYet.Count);
            for (int i = 0; i < world.ObjectsThatHaventStartedYet.Count; i++)
            {
                serializableWorld.ObjectsThatHaventStartedYet.Add(world.ObjectsThatHaventStartedYet[i].SimObjectId);
            }

            // Here we store all the base info for all entities
            serializableWorld.Entities = new List<SimSerializableWorld.Entity>(world.Entities.Count);

            // In this we store all the custom component data
            //      (For every entity, we have 1 data stack. Each component may take n entries of that stack (generally 1 per inheritance level))

            foreach (SimEntity simEntity in world.Entities)
            {
                DebugService.Log($"Serializing entity {simEntity.gameObject.name}");

                // define new serialized entity
                SimSerializableWorld.Entity serializableEntity = new SimSerializableWorld.Entity();
                serializableEntity.Active = simEntity.gameObject.activeSelf;
                serializableEntity.BlueprintIdIndex = blueprintIdIndexMap.GetIndexFromBlueprintId(simEntity.BlueprintId);
                serializableEntity.Id = simEntity.SimObjectId;
                serializableEntity.Name = simEntity.gameObject.name; // maybe we could remove this
                serializableEntity.Components = new List<SimSerializableWorld.Entity.Component>();

                // Fill component data
                simEntity.GetComponents(simComponentList);
                foreach (SimComponent simComponent in simComponentList)
                {
                    // Fill component base info
                    serializableEntity.Components.Add(new SimSerializableWorld.Entity.Component()
                    {
                        Enabled = simComponent.enabled,
                        Id = simComponent.SimObjectId,
                        Type = simComponent.GetType()
                    });


                    // Here, we ask the component to give us its custom data (position, health, events, etc.)

                    try
                    {
                        simComponent.SerializeToDataStack(componentDataStack);
                    }
                    catch (Exception e)
                    {
                        DebugService.LogError($"Failed to add SerializableData to data stack on {simComponent.gameObject.name}'s {simComponent.name}." +
                            $" Error: {e.Message} " +
                            $" Stack: {e.StackTrace}");
                        PartialSuccess = true;
                    }
                }

                // add to the entity list
                serializableWorld.Entities.Add(serializableEntity);
            }


            DebugService.Log($"Serializing Component Data Stack ...");

            {
                // Serialize the component data stack right away, and 
                //  store that data stream inside the serializableEntity (that will be reserialized in the simSerializableWorld)
                JsonSerializationUtility.SerializationResult result = JsonSerializationUtility.Serialize(
                    componentDataStack,
                    _jsonSerializerSettings,
                    Formatting.None);

                if (result.Success)
                {
                    serializableWorld.SerializedComponentDataStack = result.Json;
                }
                else
                {
                    serializableWorld.SerializedComponentDataStack = "";
                    DebugService.LogError($"Failed to serialize component data stack.\n{result.ErrorMessage}");
                    TerminateWithFailure();
                    yield break;
                }
            }

            DebugService.Log($"-- done with entities --");
            DebugService.Log($"Serializing SimSerializableWorld");

            ////////////////////////////////////////////////////////////////////////////////////////
            //      Serialize serializableWorld to string
            ////////////////////////////////////////////////////////////////////////////////////////
            serializableWorld.ReferencedBlueprints = blueprintIdIndexMap.GetList();
            yield return JsonSerializationUtility.SerializeThreaded(
                serializableWorld,
                _jsonSerializerSettings,
                Formatting.Indented, // indented json formating, for readability only
                (JsonSerializationUtility.SerializationResult result) =>
                {
                    if (result.Success)
                    {
                        SerializationData = result.Json;
                        DebugService.Log($"Serialization complete - tick: {serializableWorld.TickId}    byte size: {result.Json.Length * sizeof(char)}    kilobyte size: {result.Json.Length * sizeof(char) / 1024}");

                        if (s_printSerializeResult.BoolValue)
                        {
                            DebugService.Log(result.Json);
                        }

                        TerminateWithSuccess();
                    }
                    else
                    {
                        DebugService.LogError($"Failed to serialize SimSerializableWorld.\n{result.ErrorMessage}");

                        TerminateWithFailure();
                    }
                });
        }

        protected override void OnTerminate()
        {
            _simObjectJsonConverter.SimObjectsReferenceTable = null;
            _simObjectJsonConverter.BlueprintIdIndexMap = null;
        }
    }
}
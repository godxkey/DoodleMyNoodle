using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class SimModuleSerializer : SimModuleBase
{
    internal bool CanSimWorldBeSaved =>
        SimModules._SceneLoader.PendingSceneLoads == 0
        && SimModules._Ticker.IsTicking == false
        && IsInDeserializationProcess == false;

    internal bool IsInDeserializationProcess;
    internal bool IsInSerializationProcess;

    JsonSerializerSettings _jsonSettings;
    SimObjectJsonConverter _simObjectJsonConverter;

    JsonSerializerSettings GetJsonSettings()
    {
        if (_jsonSettings == null)
        {
            _simObjectJsonConverter = new SimObjectJsonConverter();

            _jsonSettings = new JsonSerializerSettings();

            _jsonSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            _jsonSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            _jsonSettings.TypeNameHandling = TypeNameHandling.Auto;
            _jsonSettings.Converters = new List<JsonConverter>()
            {
                _simObjectJsonConverter,
                new IDTypeJsonConverter(),
                new Fix64JsonConverter(),
            };
            _jsonSettings.ContractResolver = new CustomJsonContractResolver();
            _jsonSettings.TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
        }
        return _jsonSettings;
    }

    SimObjectJsonConverter GetSimObjectJsonConverter()
    {
        GetJsonSettings();
        return _simObjectJsonConverter;
    }

    public void SerializeSimulation(Action<string> onComplete)
    {
        if (!CanSimWorldBeSaved)
        {
            DebugService.LogError("Cannot serialize SimWorld right now. We must not be ticking nor loading a scene");
            onComplete("");
        }

        CoroutineLauncherService.Instance.StartCoroutine(SerializationRoutine(onComplete));
    }

    IEnumerator SerializationRoutine(Action<string> onComplete)
    {
        IsInSerializationProcess = true;

        // utility variables
        List<SimComponent> simComponentList = new List<SimComponent>();
        SimComponentDataStack componentDataStack = new SimComponentDataStack();
        SimBlueprintIdIndexMap blueprintIdIndexMap = new SimBlueprintIdIndexMap();
        GetSimObjectJsonConverter().BlueprintIdIndexMap = blueprintIdIndexMap; // necessary for json serialization
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
                    Id = simComponent.SimObjectId
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
                }
            }


            // Serialize the component data stack right away, and 
            //  store that data stream inside the serializableEntity (that will be reserialized in the simSerializableWorld)
            //      NB: The 'yield return' will cause unity to skip 1 frame. The serialization here is done on the main thread 
            //          because our SimObjectJsonConverter uses some unity methods that need to be done on the main thread.
            yield return null;
            JsonSerializationUtility.SerializationResult result = JsonSerializationUtility.Serialize(
                componentDataStack,
                GetJsonSettings(),
                Formatting.None);

            if (result.Success)
            {
                serializableEntity.SerializedComponentDataStack = result.Json;
            }
            else
            {
                serializableEntity.SerializedComponentDataStack = "";
                DebugService.LogError($"Failed to serialize component data stack for {serializableEntity.Name}.\n{result.ErrorMessage}");
            }

            // add to the entity list
            serializableWorld.Entities.Add(serializableEntity);

            // Clear data stack object. We'll reuse it for the next entities
            componentDataStack.Clear();
        }

        DebugService.Log($"-- done with entities --");
        DebugService.Log($"Serializing SimSerializableWorld");

        ////////////////////////////////////////////////////////////////////////////////////////
        //      Serialize serializableWorld to string
        ////////////////////////////////////////////////////////////////////////////////////////
        serializableWorld.ReferencedBlueprints = blueprintIdIndexMap.GetList();
        yield return JsonSerializationUtility.SerializeThreaded(
            serializableWorld,
            GetJsonSettings(),
            Formatting.Indented, // indented json formating, for readability only
            (JsonSerializationUtility.SerializationResult result) =>
        {
            IsInSerializationProcess = false;
            if (result.Success)
            {
                onComplete.SafeInvoke(result.Json);
            }
            else
            {
                DebugService.LogError($"Failed to serialize SimSerializableWorld.\n{result.ErrorMessage}");
                onComplete.SafeInvoke("");
            }
        });

        // clean up
        GetSimObjectJsonConverter().SimObjectsReferenceTable = null;
        GetSimObjectJsonConverter().BlueprintIdIndexMap = null;
    }

    public void DeserializeSimulation(string data, Action onComplete)
    {
        if (!CanSimWorldBeSaved)
        {
            DebugService.LogError("Cannot deserialize SimWorld right now. We must not be ticking nor loading a scene");
            return;
        }

        CoroutineLauncherService.Instance.StartCoroutine(DeserializationRoutine(data, onComplete));
    }

    IEnumerator DeserializationRoutine(string data, Action onComplete)
    {
        IsInDeserializationProcess = true;
        SimSerializableWorld serializableWorld = null;

        DebugService.Log("Start Deserialization Process...");
        // Deserialize the json data on a thread
        //  'yield return' will cause unity to continue to the next frame until the deserialization is done
        yield return JsonSerializationUtility.DeserializeThreaded<SimSerializableWorld>(data, GetJsonSettings(),
            (JsonSerializationUtility.DeserializationResult result) =>
        {
            if (result.Success)
            {
                serializableWorld = (SimSerializableWorld)result.Object;
                DebugService.Log($"SerializableSimWorld correctly deserialized");
            }
            else
            {
                DebugService.LogError($"Failed to deserialize SimSerializableWorld.\n{result.ErrorMessage}");
                serializableWorld = null;
            }
        });

        if (serializableWorld == null)
        {
            IsInDeserializationProcess = false;
            yield break;
        }

        SimWorld world = SimModules._World;

        ////////////////////////////////////////////////////////////////////////////////////////
        //      Delete existing entites
        ////////////////////////////////////////////////////////////////////////////////////////
        DebugService.Log($"Destroying existing entities");
        ClearCurrentEntities(world);


        ////////////////////////////////////////////////////////////////////////////////////////
        //      Begin filling data in SimWorld
        ////////////////////////////////////////////////////////////////////////////////////////
        DebugService.Log($"Filling new world basic info");
        world.TickId = serializableWorld.TickId;
        world.Seed = serializableWorld.Seed;
        world.NextObjectId = serializableWorld.NextObjectId;

        ////////////////////////////////////////////////////////////////////////////////////////
        //      Aquire necessary blueprints to reconstruct entites
        ////////////////////////////////////////////////////////////////////////////////////////
        DebugService.Log($"Aquiring necessary blueprints to reconstruct entites...");
        SimBlueprint[] resultBlueprints = new SimBlueprint[serializableWorld.Entities.Count];

        yield return SimModules._BlueprintManager.ProvideBlueprintBatched(serializableWorld.ReferencedBlueprints, (x) => resultBlueprints = x);

        SimBlueprint[] bp = resultBlueprints;

        ////////////////////////////////////////////////////////////////////////////////////////
        //      Reconstruct entities
        ////////////////////////////////////////////////////////////////////////////////////////
        DebugService.Log($"Reconstructing entites...");
        int reconstructCount = resultBlueprints.Length;
        Dictionary<SimObjectId, SimObject> allSimObjects = new Dictionary<SimObjectId, SimObject>(reconstructCount * 4); // expecting ~4 components per entity
        string[] serializedComponentDataStacks = new string[reconstructCount];
        SimComponentDataStack[] componentDataStacks = new SimComponentDataStack[reconstructCount];
        List<SimComponent> componentList = new List<SimComponent>();

        for (int i = 0; i < serializableWorld.Entities.Count; i++)
        {
            SimEntity reconstructedEntity = null;
            SimSerializableWorld.Entity serializedEntity = serializableWorld.Entities[i];

            SimBlueprint blueprint = resultBlueprints.TryGetAt(serializedEntity.BlueprintIdIndex);

            if (blueprint.IsValid)
            {
                reconstructedEntity = blueprint.Prefab.DuplicateGO();
                reconstructedEntity.SimObjectId = serializedEntity.Id;
                reconstructedEntity.gameObject.name = serializedEntity.Name;
                reconstructedEntity.gameObject.SetActive(serializedEntity.Active);
                reconstructedEntity.BlueprintId = blueprint.Id; // is this necessary ?
                reconstructedEntity.GetComponents(componentList);
                for (int c = 0; c < componentList.Count; c++)
                {
                    if (c < serializedEntity.Components.Count)
                    {
                        componentList[c].SimObjectId = serializedEntity.Components[c].Id;
                        componentList[c].enabled = serializedEntity.Components[c].Enabled;
                    }
                    else
                    {
                        DebugService.LogWarning($"The reconstructed entity {reconstructedEntity.gameObject.name} has a " +
                            $"component({componentList[c].GetType()}) that was not found in the serialized simulation. " +
                            $"It may be a new component that was not there when the serialization happened.");
                    }

                    // cache SimObject
                    allSimObjects.Add(componentList[c].SimObjectId, componentList[c]);
                }

                // get serialized component data stack. This will be used to refill the components with data
                serializedComponentDataStacks[i] = serializedEntity.SerializedComponentDataStack;

                // cache SimObject
                allSimObjects.Add(reconstructedEntity.SimObjectId, reconstructedEntity);
            }
            else
            {
                Debug.LogWarning($"Invalid blueprint (index:{i}), entity '{serializableWorld.Entities[i].Name}' reconstruction skipped");
            }

            // We might add a null entity. We do that to maintain index parity between world.Entites and serializedWorld.Entities
            // We clear null entities afterwards.
            world.Entities.Add(reconstructedEntity);
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        //      Release blueprints that were loaded for the reconstructions
        ////////////////////////////////////////////////////////////////////////////////////////
        DebugService.Log($"Releaseing blueprints loaded for reconstruction...");
        SimModules._BlueprintManager.ReleaseBatchedBlueprints();

        ////////////////////////////////////////////////////////////////////////////////////////
        //      Deserialize all component data stacks
        ////////////////////////////////////////////////////////////////////////////////////////
        DebugService.Log($"Deserializing component data stacks...");
        GetSimObjectJsonConverter().SimObjectsReferenceTable = allSimObjects; // necessary for json to reassign references correctly
        GetSimObjectJsonConverter().AvailableBlueprints = resultBlueprints;

        // NB: cannot be threaded for now because our json converter accesses Unity API which is main thread bound. Could probably be changed!
        yield return JsonSerializationUtility.DeserializeBatch<SimComponentDataStack>( 
            serializedComponentDataStacks,
            GetJsonSettings(),
            (JsonSerializationUtility.DeserializationResult[] results) =>
        {
            for (int i = 0; i < results.Length; i++)
            {
                if (results[i].Success)
                {
                    componentDataStacks[i] = (SimComponentDataStack)results[i].Object;
                }
                else
                {
                    DebugService.LogError($"Failed to deserialize SimComponentDataStack for entity {serializableWorld.Entities[i].Name}.\n{results[i].ErrorMessage}");
                }
            }
        });

        ////////////////////////////////////////////////////////////////////////////////////////
        //      Refill components with data (from data stacks)
        ////////////////////////////////////////////////////////////////////////////////////////
        DebugService.Log($"Refilling components' data...");
        for (int i = 0; i < serializableWorld.Entities.Count; i++)
        {
            SimEntity reconstructedEntity = world.Entities[i];
            SimComponentDataStack dataStack = componentDataStacks[i];
            if (reconstructedEntity != null && dataStack != null) // skip over invalid entities
            {
                reconstructedEntity.GetComponents(componentList);
                for (int c = 0; c < componentList.Count; c++)
                {
                    try
                    {
                        componentList[c].DeserializeFromDataStack(componentDataStacks[i]);
                    }
                    catch (Exception e)
                    {
                        DebugService.LogWarning($"Failed to deserialize {reconstructedEntity.gameObject.name}'s {componentList[c].GetType()} component: {e.Message}." +
                            $"\nComponent data will stay at default values.");
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        //      Remove null entities
        ////////////////////////////////////////////////////////////////////////////////////////
        DebugService.Log($"Clearing entities that failed to be reconstructed...");
        world.Entities.RemoveNulls();

        ////////////////////////////////////////////////////////////////////////////////////////
        //      Add all reconstructed entities to runtime
        ////////////////////////////////////////////////////////////////////////////////////////
        DebugService.Log($"Adding reconstructed entities to runtime...");
        world.Entities.ForEach((x) => SimModules._EntityManager.AddEntityToRuntime(x));

        // terminado!
        DebugService.Log($"Deserialization complete!");
        IsInDeserializationProcess = false;
        onComplete.SafeInvoke();
    }

    void ClearCurrentEntities(SimWorld world)
    {
        for (int i = 0; i < world.Entities.Count; i++)
        {
            SimModules._EntityManager.RemoveEntityFromRuntime(world.Entities[i]);
            world.Entities[i].DestroyGO();
        }
        world.Entities.Clear();
    }
}
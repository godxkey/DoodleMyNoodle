using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.SceneManagement;

public class SimulationWorldSystem : ComponentSystem, IWorldOwner
{
    List<string> _incomingEntityInjections = new List<string>();

    World IWorldOwner.OwnedWorld => SimulationWorld;
    public SimulationWorld SimulationWorld { get; private set; }
    public World PresentationWorld => World;

    protected override void OnCreate()
    {
        base.OnCreate();

        SimulationWorld = new SimulationWorld("Simulation World");
        SimulationWorld.Owner = this;

        World.GetOrCreateSystem<SimulationLoadSceneSystem>();
        World.GetOrCreateSystem<SimulationWorldUpdaterSystem>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (SimulationWorld.IsCreated)
            SimulationWorld.Dispose();

        SimulationWorld = null;
    }

    protected override void OnUpdate()
    {
    }


    public void RegisterIncomingEntityInjection(string sceneName)
    {
        _incomingEntityInjections.Add(sceneName);
    }

    public void UnregisterIncomingEntityInjection(string sceneName)
    {
        _incomingEntityInjections.Remove(sceneName);
    }

    void IWorldOwner.OnBeginEntitiesInjectionFromGameObjectConversion(List<Scene> comingFromScenes)
    {
        foreach (var scene in comingFromScenes)
        {
            if(!_incomingEntityInjections.Contains(scene.name))
            {
                DebugService.LogError($"Unexpected entities coming from {scene.name} are being injected into the simulation. " +
                    $"This should not happen");
            }
        }

        var changeDetectionEnd = SimulationWorld.GetExistingSystem<ChangeDetectionSystemEnd>();
        if (changeDetectionEnd != null)
        {
            changeDetectionEnd.ForceEndSample();
        }
    }

    void IWorldOwner.OnEndEntitiesInjectionFromGameObjectConversion()
    {
        var changeDetectionBegin = SimulationWorld.GetExistingSystem<ChangeDetectionSystemBegin>();
        if (changeDetectionBegin != null)
        {
            changeDetectionBegin.ResetSample();
        }
    }

    public void ClearSimWorld()
    {
        DebugService.Log($"Clearing {SimulationWorld.Name} ...");
        var changeDetectionEnd = SimulationWorld.GetExistingSystem<ChangeDetectionSystemEnd>();
        var changeDetectionBegin = SimulationWorld.GetExistingSystem<ChangeDetectionSystemBegin>();

        if (changeDetectionEnd != null)
        {
            changeDetectionEnd.ForceEndSample();
        }

        SimulationWorld.EntityManager.DestroyEntity(SimulationWorld.EntityManager.UniversalQuery);

        if (changeDetectionBegin != null)
        {
            changeDetectionBegin.ResetSample();
        }
    }

    //private void SetChangeDetectionLogMode(ChangeDetectionSystemEnd.LogMode logMode)
    //{
    //    var changeDetectionEnd = SimulationWorld.GetExistingSystem<ChangeDetectionSystemEnd>();
    //    if (changeDetectionEnd != null)
    //    {
    //        changeDetectionEnd.LoggingMode = logMode;
    //    }
    //}

    public static void ClearAllSimulationWorlds()
    {
        foreach (World world in World.AllWorlds)
        {
            var simWorldOwner = world.GetExistingSystem<SimulationWorldSystem>();
            if (simWorldOwner != null)
            {
                simWorldOwner.ClearSimWorld();
            }
        }
    }
}


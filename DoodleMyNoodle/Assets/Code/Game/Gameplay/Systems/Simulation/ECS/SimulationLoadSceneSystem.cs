using System.Collections.Generic;
using Unity.Entities;
using UnityEngine.SceneManagement;

public class SimulationLoadSceneSystem : ComponentSystem
{
    private List<ISceneLoadPromise> _sceneLoads = new List<ISceneLoadPromise>();
    private SimulationWorldSystem _simulationWorldSystem;
    private SimulationWorldUpdaterSystem _simulationWorldUpdaterSystem;

    public ReadOnlyList<ISceneLoadPromise> OngoingSceneLoads => _sceneLoads.AsReadOnlyNoAlloc();

    protected override void OnCreate()
    {
        base.OnCreate();

        _simulationWorldSystem = World.GetOrCreateSystem<SimulationWorldSystem>();
        _simulationWorldUpdaterSystem = World.GetOrCreateSystem<SimulationWorldUpdaterSystem>();
    }

    protected override void OnUpdate()
    {
        // nothing to do
    }

    public void UpdateSceneLoading()
    {
        // REVERT
        //if (_simulationWorldUpdaterSystem.AvailableTicks.Count > 0)
        //{
        //    SimTickData tick = _simulationWorldUpdaterSystem.AvailableTicks.First();

        //    foreach (SimInput input in tick.inputs)
        //    {
        //        if (input is SimCommandLoadScene loadSceneCommand)
        //        {
        //            if (!WasAddedToSceneLoads(loadSceneCommand.SceneName))
        //            {
        //                _sceneLoads.Add(SceneService.Load(loadSceneCommand.SceneName, LoadSceneMode.Additive, LocalPhysicsMode.Physics3D));
        //            }
        //        }
        //    }
        //}

        //// clear when all scenes are loaded
        //if(AreAllLoadsComplete())
        //{
        //    _sceneLoads.Clear();
        //}
    }

    bool WasAddedToSceneLoads(string sceneName)
    {
        foreach (ISceneLoadPromise scenePromise in _sceneLoads)
        {
            if (scenePromise.SceneName == sceneName)
                return true;
        }
        return false;
    }
    
    bool AreAllLoadsComplete()
    {
        foreach (ISceneLoadPromise scenePromise in _sceneLoads)
        {
            if (!scenePromise.IsComplete)
                return false;
        }
        return true;
    }
}

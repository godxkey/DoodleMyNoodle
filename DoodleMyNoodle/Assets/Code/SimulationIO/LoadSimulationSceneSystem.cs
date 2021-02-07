using System.Collections.Generic;
using Unity.Entities;
using UnityEngine.SceneManagement;
using UnityEngineX;

namespace SimulationControl
{
    [DisableAutoCreation]
    [UpdateBefore(typeof(TickSimulationSystem))]
    [UpdateInGroup(typeof(SimulationControlSystemGroup))]
    public class LoadSimulationSceneSystem : ComponentSystem
    {
        private List<SimCommandLoadScene> _inputsToIgnore = new List<SimCommandLoadScene>();
        private List<ISceneLoadPromise> _sceneLoads = new List<ISceneLoadPromise>();
        private List<ISceneLoadPromise> _sceneLoadsToUnregisterOnUpdate = new List<ISceneLoadPromise>();
        private SimulationWorldSystem _simulationWorldSystem;
        private TickSimulationSystem _tickSystem;
        private DirtyValue<uint> _tickId;

        public ReadOnlyList<ISceneLoadPromise> OngoingSceneLoads => _sceneLoads.AsReadOnlyNoAlloc();

        protected override void OnCreate()
        {
            base.OnCreate();

            _simulationWorldSystem = World.GetOrCreateSystem<SimulationWorldSystem>();
            _tickSystem = World.GetOrCreateSystem<TickSimulationSystem>();
        }

        protected override void OnUpdate()
        {

            foreach (var item in _sceneLoadsToUnregisterOnUpdate)
            {
                _simulationWorldSystem.UnregisterIncomingEntityInjection(item.SceneName);
            }
            _sceneLoadsToUnregisterOnUpdate.Clear();
        }

        public void UpdateSceneLoading()
        {
            if (_tickSystem.AvailableTicks.Count > 0)
            {
                SimTickData tick = _tickSystem.AvailableTicks.First();

                _tickId.Set(tick.ExpectedNewTickId); // clear ignore list if tick has changed
                if (_tickId.ClearDirty())
                {
                    _inputsToIgnore.Clear();
                }

                foreach (SimInputSubmission inputSubmission in tick.InputSubmissions)
                {
                    if (inputSubmission.Input is SimCommandLoadScene loadSceneCommand)
                    {
                        if (!IsLoading(loadSceneCommand.SceneName) && !_inputsToIgnore.Contains(loadSceneCommand))
                        {
                            // add input to 'ignore list' to make sure we don't load the same scenes over and over
                            _inputsToIgnore.Add(loadSceneCommand);

                            // prepare for entity injection (through convertion)
                            _simulationWorldSystem.RegisterIncomingEntityInjection(loadSceneCommand.SceneName);

                            // request load
                            _sceneLoads.Add(SceneService.Load(loadSceneCommand.SceneName, LoadSceneMode.Additive, LocalPhysicsMode.Physics3D));
                        }
                    }
                }
            }

            // clear when all scenes are loaded
            if (AreAllLoadsComplete())
            {
                _sceneLoadsToUnregisterOnUpdate.AddRange(_sceneLoads);
                _sceneLoads.Clear();
            }
        }

        bool IsLoading(string sceneName)
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

}
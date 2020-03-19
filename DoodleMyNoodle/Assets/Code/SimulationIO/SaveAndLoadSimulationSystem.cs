using CCC.Operations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace SimulationControl
{
    [DisableAutoCreation]
    [UpdateBefore(typeof(TickSimulationSystem))]
    [UpdateInGroup(typeof(SimulationControlSystemGroup))]
    public class SaveAndLoadSimulationSystem : ComponentSystem
    {
        private CoroutineOperation _ongoingCmdOperation;
        private SimulationWorldSystem _simulationWorldSystem;
        private TickSimulationSystem _tickSystem;

        // !! CONSIDER SPLITING THIS SYSTEM IN TWO (one for load, one for save)!!

        protected override void OnCreate()
        {
            base.OnCreate();

            GameConsole.AddCommand("sim.save", Cmd_SimSave, "Save the simulation in memory (multiplayer unsafe!)");
            GameConsole.AddCommand("sim.load", Cmd_SimLoad, "Load the simulation from memory (multiplayer unsafe!)");

            _simulationWorldSystem = World.GetOrCreateSystem<SimulationWorldSystem>();
            _tickSystem = World.GetOrCreateSystem<TickSimulationSystem>();
        }

        protected override void OnDestroy()
        {
            GameConsole.RemoveCommand("sim.save");
            GameConsole.RemoveCommand("sim.load");

            if (_ongoingCmdOperation != null && _ongoingCmdOperation.IsRunning)
                _ongoingCmdOperation.TerminateWithFailure();

            base.OnDestroy();
        }

        protected override void OnUpdate()
        {
        }

#if DEBUG_BUILD
        private void Cmd_SimSave(string[] parameters)
        {
            if (_ongoingCmdOperation != null && _ongoingCmdOperation.IsRunning)
            {
                Debug.LogWarning("Cannot SaveSim because another operation is ongoing");
                return;
            }

            // pick operation to launch
            string locationTxt;
            if (parameters.Length > 0)
            {
                string fileNameToSaveTo = parameters[0];
                if (!fileNameToSaveTo.EndsWith(".txt"))
                {
                    fileNameToSaveTo += ".txt";
                }

                _ongoingCmdOperation = new SaveSimulationToDiskOperation($"{Application.persistentDataPath}/{fileNameToSaveTo}", _simulationWorldSystem.SimulationWorld);
                locationTxt = $"file {fileNameToSaveTo}";
            }
            else
            {
                _ongoingCmdOperation = new SaveSimulationToMemoryOperation(_simulationWorldSystem.SimulationWorld);
                locationTxt = "memory";
            }

            _tickSystem.PauseSimulation("save_cmd");

            _ongoingCmdOperation.OnTerminateCallback = (op) => _tickSystem.UnpauseSimulation("save_cmd");

            _ongoingCmdOperation.OnSucceedCallback = (op) =>
            {
                DebugScreenMessage.DisplayMessage($"Saved sim to {locationTxt}. {op.Message}");
            };

            _ongoingCmdOperation.OnFailCallback = (op) =>
            {
                DebugScreenMessage.DisplayMessage($"Could not save sim to {locationTxt}. {op.Message}");
            };

            // start operation
            _ongoingCmdOperation.Execute();
        }

        private void Cmd_SimLoad(string[] parameters)
        {
            if (_ongoingCmdOperation != null && _ongoingCmdOperation.IsRunning)
            {
                Debug.LogWarning("Cannot LoadSim because another operation is ongoing");
                return;
            }

            string locationTxt;

            if (parameters.Length > 0)
            {
                string fileNameToReadFrom = parameters[0];
                if (!fileNameToReadFrom.EndsWith(".txt"))
                {
                    fileNameToReadFrom += ".txt";
                }

                _ongoingCmdOperation = new LoadSimulationFromDiskOperation(
                    $"{Application.persistentDataPath}/{fileNameToReadFrom}",
                    _simulationWorldSystem.SimulationWorld);
                locationTxt = $"file {fileNameToReadFrom}";
            }
            else
            {
                _ongoingCmdOperation = new LoadSimulationFromMemoryOperation(_simulationWorldSystem.SimulationWorld);
                locationTxt = "memory";
            }

            _tickSystem.PauseSimulation("load_cmd");

            _ongoingCmdOperation.OnTerminateCallback = (op) => _tickSystem.UnpauseSimulation("load_cmd");

            _ongoingCmdOperation.OnSucceedCallback = (op) =>
            {
                DebugScreenMessage.DisplayMessage($"Loaded sim from {locationTxt}. {op.Message}");
            };

            _ongoingCmdOperation.OnFailCallback = (op) =>
            {
                DebugScreenMessage.DisplayMessage($"Could not load sim from {locationTxt}. {op.Message}");
            };

            _ongoingCmdOperation.Execute();
        }
#endif
    }
}

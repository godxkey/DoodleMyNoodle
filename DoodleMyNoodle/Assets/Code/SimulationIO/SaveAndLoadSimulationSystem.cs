using CCC.Operations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

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

#if DEBUG
            s_commandInstance = this;
            GameConsole.SetCommandOrVarEnabled("Sim.SaveToMemory", true);
            GameConsole.SetCommandOrVarEnabled("Sim.SaveToFile", true);
            GameConsole.SetCommandOrVarEnabled("Sim.LoadFromMemory", true);
            GameConsole.SetCommandOrVarEnabled("Sim.LoadFromFile", true);
#endif

            _simulationWorldSystem = World.GetOrCreateSystem<SimulationWorldSystem>();
            _tickSystem = World.GetOrCreateSystem<TickSimulationSystem>();
        }

        protected override void OnDestroy()
        {
#if DEBUG
            GameConsole.SetCommandOrVarEnabled("Sim.SaveToMemory", false);
            GameConsole.SetCommandOrVarEnabled("Sim.SaveToFile", false);
            GameConsole.SetCommandOrVarEnabled("Sim.LoadFromMemory", false);
            GameConsole.SetCommandOrVarEnabled("Sim.LoadFromFile", false);
            s_commandInstance = null;
#endif

            if (_ongoingCmdOperation != null && _ongoingCmdOperation.IsRunning)
                _ongoingCmdOperation.TerminateWithNormalFailure();

            base.OnDestroy();
        }

        protected override void OnUpdate()
        {
        }

#if DEBUG
        private static SaveAndLoadSimulationSystem s_commandInstance;

        [ConsoleCommand("Sim.SaveToMemory", "Save the simulation in memory (multiplayer unsafe!)", EnabledByDefault = false)]
        private static void Cmd_SimSaveToMemory()
        {
            if (s_commandInstance._ongoingCmdOperation != null && s_commandInstance._ongoingCmdOperation.IsRunning)
            {
                Debug.LogWarning("Cannot SaveSim because another operation is ongoing");
                return;
            }

            s_commandInstance._ongoingCmdOperation = new SaveSimulationToMemoryOperation(s_commandInstance._simulationWorldSystem.SimulationWorld);
            Cmd_PostSimSave_Internal(locationTxt: "memory");
        }

        [ConsoleCommand("Sim.SaveToFile", "Save the simulation to a text file (multiplayer unsafe!)", EnabledByDefault = false)]
        private static void Cmd_SimSaveToFile(string fileNameToSaveTo)
        {
            if (s_commandInstance._ongoingCmdOperation != null && s_commandInstance._ongoingCmdOperation.IsRunning)
            {
                Debug.LogWarning("Cannot SaveSim because another operation is ongoing");
                return;
            }

            if (!fileNameToSaveTo.EndsWith(".txt"))
            {
                fileNameToSaveTo += ".txt";
            }

            s_commandInstance._ongoingCmdOperation = new SaveSimulationToDiskOperation($"{Application.persistentDataPath}/{fileNameToSaveTo}", s_commandInstance._simulationWorldSystem.SimulationWorld);

            Cmd_PostSimSave_Internal(locationTxt: $"file {fileNameToSaveTo}");
        }

        private static void Cmd_PostSimSave_Internal(string locationTxt)
        {
            s_commandInstance._tickSystem.PauseSimulation("save_cmd");

            s_commandInstance._ongoingCmdOperation.OnTerminateCallback = (op) => s_commandInstance._tickSystem.UnpauseSimulation("save_cmd");

            s_commandInstance._ongoingCmdOperation.OnSucceedCallback = (op) =>
            {
                DebugScreenMessage.DisplayMessage($"Saved sim to {locationTxt}. {op.Message}");
            };

            s_commandInstance._ongoingCmdOperation.OnFailCallback = (op) =>
            {
                DebugScreenMessage.DisplayMessage($"Could not save sim to {locationTxt}. {op.Message}");
            };

            // start operation
            s_commandInstance._ongoingCmdOperation.Execute();
        }

        [ConsoleCommand("Sim.LoadFromMemory", "Load the simulation from memory (multiplayer unsafe!)", EnabledByDefault = false)]
        private static void Cmd_SimLoadFromMemory(string fileNameToReadFrom)
        {
            if (s_commandInstance._ongoingCmdOperation != null && s_commandInstance._ongoingCmdOperation.IsRunning)
            {
                Log.Warning("Cannot load sim because another operation is ongoing");
                return;
            }

            SimulationWorld newWorld = s_commandInstance._simulationWorldSystem.CreateNewReplacementWorld();

            s_commandInstance._ongoingCmdOperation = new LoadSimulationFromMemoryOperation(newWorld);
            Cmd_PostSimLoad_Internal(locationTxt: "memory", newWorld);
        }

        [ConsoleCommand("Sim.LoadFromFile", "Load the simulation from a text file (multiplayer unsafe!)", EnabledByDefault = false)]
        private static void Cmd_SimLoadFromFile(string fileName)
        {
            if (s_commandInstance._ongoingCmdOperation != null && s_commandInstance._ongoingCmdOperation.IsRunning)
            {
                Log.Warning("Cannot load sim because another operation is ongoing");
                return;
            }

            if (!fileName.EndsWith(".txt"))
            {
                fileName += ".txt";
            }

            SimulationWorld newWorld = s_commandInstance._simulationWorldSystem.CreateNewReplacementWorld();

            s_commandInstance._ongoingCmdOperation = new LoadSimulationFromDiskOperation(
                $"{Application.persistentDataPath}/{fileName}",
                newWorld);

            Cmd_PostSimLoad_Internal(locationTxt: $"file {fileName}", newWorld);
        }

        private static void Cmd_PostSimLoad_Internal(string locationTxt, SimulationWorld newWorld)
        {
            s_commandInstance._tickSystem.PauseSimulation("load_cmd");

            s_commandInstance._ongoingCmdOperation.OnTerminateCallback = (op) => s_commandInstance._tickSystem.UnpauseSimulation("load_cmd");

            s_commandInstance._ongoingCmdOperation.OnSucceedCallback = (op) =>
            {
                DebugScreenMessage.DisplayMessage($"Loaded sim from {locationTxt}. {op.Message}");

                s_commandInstance._simulationWorldSystem.RequestReplaceSimWorld(newWorld);
            };

            s_commandInstance._ongoingCmdOperation.OnFailCallback = (op) =>
            {
                DebugScreenMessage.DisplayMessage($"Could not load sim from {locationTxt}. {op.Message}");
            };

            s_commandInstance._ongoingCmdOperation.Execute();
        }
#endif
    }
}

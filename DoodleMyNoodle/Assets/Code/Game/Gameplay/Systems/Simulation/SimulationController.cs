using CCC.Operations;
using SimulationControl;
using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SimulationController : GameSystem<SimulationController>
{
    //[SerializeField] SimBlueprintProviderPrefab _bpProviderPrefab;
    //[SerializeField] SimBlueprintProviderSceneObject _bpProviderSceneObject;

    //private Blocker _playSimulation = new Blocker();

    //public bool CanTickSimulation => SimulationView.CanBeTicked && _playSimulation;

    public override bool SystemReady => true;

    //public abstract void SubmitInput(SimInput input);

    public override void OnGameReady()
    {
        base.OnGameReady();

        Time.fixedDeltaTime = (float)SimulationConstants.TIME_STEP;
        
        World.DefaultGameObjectInjectionWorld
            .GetOrCreateSystem<SimulationControlSystemGroup>()
            .Initialize(OnlineService.OnlineInterface?.SessionInterface, ValidateSimInput);

        //        //SimulationCoreSettings settings = new SimulationCoreSettings();
        //        //settings.BlueprintProviders = new List<ISimBlueprintProvider>()
        //        //{
        //        //    _bpProviderPrefab,
        //        //    _bpProviderSceneObject
        //        //};

        //        //SimulationView.Initialize(settings);



        ////#if DEBUG_BUILD
        ////        GameConsole.AddCommand("sim.pause", Cmd_SimPause, "Pause the simulation playback");
        ////        GameConsole.AddCommand("sim.save", Cmd_SimSave, "Save the simulation in memory (multiplayer unsafe!)");
        ////        GameConsole.AddCommand("sim.load", Cmd_SimLoad, "Load the simulation from memory (multiplayer unsafe!)");
        ////#endif
    }

    private bool ValidateSimInput(SimInput input, INetworkInterfaceConnection instigator)
    {
        PlayerInfo instigatorPlayer = PlayerHelpers.GetPlayerInfo(instigator);

        if (instigatorPlayer == null)
        {
            DebugService.Log($"[{nameof(SimulationController)}] We refused an input from connection {instigator.Id} " +
                $"because no player seems associated with that connection.");
            return false;
        }

        if (input is SimPlayerInput playerInput)
        {
            playerInput.SimPlayerId = instigatorPlayer.SimPlayerId;

            if (playerInput.SimPlayerId == PersistentId.Invalid)
            {
                DebugService.Log($"[{nameof(SimulationController)}] We refused {instigatorPlayer.PlayerName}'s input " +
                    $"because (s)he doesn't have a valid {nameof(instigatorPlayer.SimPlayerId)} yet.");
                return false;
            }
        }

        return true;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        World.DefaultGameObjectInjectionWorld?.GetExistingSystem<SimulationControlSystemGroup>()?.Shutdown();
        //#if DEBUG_BUILD
        //        GameConsole.RemoveCommand("sim.load");
        //        GameConsole.RemoveCommand("sim.save");
        //        GameConsole.RemoveCommand("sim.pause");

        //        if (_ongoingCmdOperation != null && _ongoingCmdOperation.IsRunning)
        //            _ongoingCmdOperation.TerminateWithFailure();
        //#endif


        //if (SimulationView.IsRunningOrReadyToRun)
        //    SimulationView.Dispose();
    }

    //    protected void PauseSimulation(string key)
    //    {
    //        _playSimulation.BlockUnique(key);
    //    }

    //    protected void UnpauseSimulation(string key)
    //    {
    //        _playSimulation.Unblock(key);
    //    }

    //    protected bool IsSimulationPaused => !_playSimulation;

    //#if DEBUG_BUILD
    //    protected CoroutineOperation _ongoingCmdOperation;
    //    void Cmd_SimSave(string[] parameters)
    //    {
    //        if (_ongoingCmdOperation != null && _ongoingCmdOperation.IsRunning)
    //        {
    //            Debug.LogWarning("Cannot SaveSim because another operation is ongoing");
    //            return;
    //        }

    //        // pick operation to launch
    //        string locationTxt;
    //        if (parameters.Length > 0)
    //        {
    //            string fileNameToSaveTo = parameters[0];
    //            if (!fileNameToSaveTo.EndsWith(".txt"))
    //            {
    //                fileNameToSaveTo += ".txt";
    //            }

    //            _ongoingCmdOperation = new SaveSimulationToDiskOperation($"{Application.persistentDataPath}/{fileNameToSaveTo}", SimulationWorld);
    //            locationTxt = $"file {fileNameToSaveTo}";
    //        }
    //        else
    //        {
    //            _ongoingCmdOperation = new SaveSimulationToMemoryOperation(SimulationWorld);
    //            locationTxt = "memory";
    //        }

    //        _ongoingCmdOperation.OnSucceedCallback = (op) =>
    //        {
    //            DebugScreenMessage.DisplayMessage($"Saved sim to {locationTxt}. {op.Message}");
    //        };

    //        _ongoingCmdOperation.OnFailCallback = (op) =>
    //        {
    //            DebugScreenMessage.DisplayMessage($"Could not save sim to {locationTxt}. {op.Message}");
    //        };

    //        // start operation
    //        _ongoingCmdOperation.Execute();
    //    }
    //    void Cmd_SimLoad(string[] parameters)
    //    {
    //        if (_ongoingCmdOperation != null && _ongoingCmdOperation.IsRunning)
    //        {
    //            Debug.LogWarning("Cannot LoadSim because another operation is ongoing");
    //            return;
    //        }

    //        string locationTxt;

    //        if (parameters.Length > 0)
    //        {
    //            string fileNameToReadFrom = parameters[0];
    //            if (!fileNameToReadFrom.EndsWith(".txt"))
    //            {
    //                fileNameToReadFrom += ".txt";
    //            }

    //            _ongoingCmdOperation = new LoadSimulationFromDiskOperation($"{Application.persistentDataPath}/{fileNameToReadFrom}", SimulationWorld);
    //            locationTxt = $"file {fileNameToReadFrom}";
    //        }
    //        else
    //        {
    //            _ongoingCmdOperation = new LoadSimulationFromMemoryOperation(SimulationWorld);
    //            locationTxt = "memory";
    //        }

    //        _ongoingCmdOperation.OnSucceedCallback = (op) =>
    //        {
    //            DebugScreenMessage.DisplayMessage($"Loaded sim from {locationTxt}. {op.Message}");
    //        };

    //        _ongoingCmdOperation.OnFailCallback = (op) =>
    //        {
    //            DebugScreenMessage.DisplayMessage($"Could not load sim from {locationTxt}. {op.Message}");
    //        };

    //        _ongoingCmdOperation.Execute();
    //    }

    //    void Cmd_SimPause(string[] args)
    //    {
    //        if (IsSimulationPaused)
    //        {
    //            UnpauseSimulation(key: "cmd");
    //        }
    //        else
    //        {
    //            PauseSimulation(key: "cmd");
    //        }
    //    }
    //#endif
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SimulationController : GameSystem<SimulationController>
{
    [SerializeField] SimBlueprintProviderPrefab _bpProviderPrefab;
    [SerializeField] SimBlueprintProviderSceneObject _bpProviderSceneObject;

    public override bool SystemReady => true;

    public abstract void SubmitInput(SimInput input);

    public override void OnGameReady()
    {
        Time.fixedDeltaTime = (float)SimulationConstants.TIME_STEP;


        SimulationCoreSettings settings = new SimulationCoreSettings();
        settings.BlueprintProviders = new List<ISimBlueprintProvider>()
        {
            _bpProviderPrefab,
            _bpProviderSceneObject
        };

        SimulationView.Initialize(settings);

        base.OnGameReady();


#if DEBUG_BUILD
        GameConsole.AddCommand("savesim", Cmd_SaveSim, "Save the simulation in memory (multiplayer unsafe!)");
        GameConsole.AddCommand("loadsim", Cmd_LoadSim, "Load the simulation from memory (multiplayer unsafe!)");
#endif
    }


    protected override void OnDestroy()
    {
#if DEBUG_BUILD
        GameConsole.RemoveCommand("loadsim");
        GameConsole.RemoveCommand("savesim");
#endif

        base.OnDestroy();

        if (SimulationView.IsRunningOrReadyToRun)
            SimulationView.Dispose();
    }

#if DEBUG_BUILD
    string _savedSimInMemory;
    void Cmd_SaveSim(string[] parameters)
    {
        SimulationView.SerializeSimulation((string result) =>
        {
            if (parameters.Length > 0)
            {
                string fileNameToSaveTo = parameters[0];
                if (!fileNameToSaveTo.EndsWith(".txt"))
                {
                    fileNameToSaveTo += ".txt";
                }

                System.IO.File.WriteAllText($@"{Application.persistentDataPath}/{fileNameToSaveTo}", result);
                DebugScreenMessage.DisplayMessage($"Sim saved in file {fileNameToSaveTo}");
            }
            else
            {
                DebugScreenMessage.DisplayMessage("Sim saved in memory");
                _savedSimInMemory = result;
            }
        });
    }
    void Cmd_LoadSim(string[] parameters)
    {
        string simData;
        string locationTxt;

        if (parameters.Length > 0)
        {
            string fileNameToReadFrom = parameters[0];
            if (!fileNameToReadFrom.EndsWith(".txt"))
            {
                fileNameToReadFrom += ".txt";
            }

            simData = System.IO.File.ReadAllText($@"{Application.persistentDataPath}/{fileNameToReadFrom}");
            locationTxt = $"file {fileNameToReadFrom}";
        }
        else
        {
            simData = _savedSimInMemory;
            locationTxt = "memory";
        }

        if(simData == null)
        {
            DebugScreenMessage.DisplayMessage($"Could not load sim. No data found in {locationTxt}");
        }
        else
        {
            SimulationView.DeserializeSimulation(simData, () =>
            {
                DebugScreenMessage.DisplayMessage($"Sim loaded from {locationTxt}");
            });
        }
    }
#endif
}

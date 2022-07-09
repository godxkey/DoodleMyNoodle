using CCC.Online;
using SimulationControl;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngineX;

[NetSerializable]
public struct SyncedValueCurrentMap
{
    public string Name;
}

public class MapManager : GameSystem<MapManager>
{
    public SceneInfo SimManagersScene;

    [FormerlySerializedAs("LevelBank")]
    public MapBank MapBank;

    private const string MAP_NOT_STARTED = "map-not-started";

    public bool IsMapStarted { get; private set; }

    public override void OnGameStart()
    {
        base.OnGameStart();

        PresentationHelpers.PresentationWorld.GetExistingSystem<TickSimulationSystem>().PauseSimulation(MAP_NOT_STARTED);

        SyncedValues.RegisterValueListener<SyncedValueCurrentMap>(OnMapSet, callImmediatelyIfValueExits: true);

        string mapToPlay = null;

        if (SyncedValues.CanWriteValues)
        {
            switch (GameStateManager.currentGameState)
            {
                case GameStateInGameServer serverGameState:
                    mapToPlay = serverGameState.MapToPlay;
                    break;

                case GameStateInGameLocal localGameState:
                    mapToPlay = localGameState.MapToPlay;
                    break;

                    // CLIENT DOESN'T GET TO CHOOSE !!
            }

            SyncedValues.SetOrCreate(new SyncedValueCurrentMap() { Name = mapToPlay });
        }
    }

    protected override void OnDestroy()
    {
        if (SyncedValues.CanWriteValues)
        {
            SyncedValues.Destroy<SyncedValueCurrentMap>();
        }

        PresentationHelpers.PresentationWorld?.GetExistingSystem<TickSimulationSystem>()?.PauseSimulation(MAP_NOT_STARTED);
        SyncedValues.UnregisterValueListener<SyncedValueCurrentMap>(OnMapSet);

        base.OnDestroy();
    }

    public void StartMap(string mapName)
    {
        if (!SystemReady)
        {
            Log.Error("MapManager system is not ready");
            return;
        }

        if (SyncedValues.CanWriteValues)
        {
            SyncedValues.SetOrCreate(new SyncedValueCurrentMap() { Name = mapName });
        }
        else
        {
            Log.Error("Client cannot start a map. The server must do that.");
        }
    }

    private void OnMapSet(SyncedValueCurrentMap newValue)
    {
        if (!string.IsNullOrEmpty(newValue.Name))
        {
            string mapName = newValue.Name;
            Map lvl = MapBank.Maps.Find((x) => x && x.name == mapName);
            if (!lvl)
            {
                Debug.LogError($"Could not start map {mapName}. It was not found in the map bank. " +
                    $"The bank is a scriptable object named MapBank.");
                return;
            }

            OnMapSet(lvl);
        }
        else
        {

            IsMapStarted = false;
        }
    }

    private void OnMapSet(Map map)
    {
        if (IsMapStarted)
        {
            Log.Error("Cannot start another map (not yet implemented)");
            return;
        }
        IsMapStarted = true;
        PresentationHelpers.PresentationWorld.GetExistingSystem<TickSimulationSystem>().UnpauseSimulation(MAP_NOT_STARTED);

        if (Game.PlayingAsMaster)
        {
            // load simulation scenes
            PresentationHelpers.SubmitInput(new SimCommandLoadScene() { SceneName = SimManagersScene.SceneName });
            foreach (SceneInfo scene in map.SimulationScenes)
            {
                PresentationHelpers.SubmitInput(new SimCommandLoadScene() { SceneName = scene.SceneName });
            }
        }

        // instantiate presentation
        Game.InstantiateGameplayPresentationSystems();
        
        // load presentation scenes
        foreach (SceneInfo scene in map.PresentationScenes)
        {
            SceneService.LoadAsync(scene.SceneName);
        }
    }
}

using CCC.Online;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NetSerializable]
public struct SyncedValueCurrentLevel
{
    public string Name;
}

public class LevelManager : GameSystem<LevelManager>
{
    public SceneInfo SimManagersScene;
    public SceneInfo SimBasePresentationScene;
    public LevelBank LevelBank;

    public override bool SystemReady => true;
    public bool IsLevelStarted { get; private set; }

    public override void OnGameReady()
    {
        base.OnGameReady();


        OnLevelSet(default);
    }

    public override void OnGameStart()
    {
        base.OnGameStart();

        SyncedValues.RegisterValueListener<SyncedValueCurrentLevel>(OnLevelSet, callImmediatelyIfValueExits: true);

        string levelToPlay = null;

        if (SyncedValues.CanWriteValues)
        {
            switch (GameStateManager.currentGameState)
            {
                case GameStateInGameServer serverGameState:
                    levelToPlay = serverGameState.LevelToPlay;
                    break;

                case GameStateInGameLocal localGameState:
                    levelToPlay = localGameState.LevelToPlay;
                    break;

                    // CLIENT DOESN'T GET TO CHOOSE !!
            }

            SyncedValues.SetOrCreate(new SyncedValueCurrentLevel() { Name = levelToPlay });
        }
    }

    protected override void OnDestroy()
    {
        if (SyncedValues.CanWriteValues)
        {
            SyncedValues.Destroy<SyncedValueCurrentLevel>();
        }

        SyncedValues.UnregisterValueListener<SyncedValueCurrentLevel>(OnLevelSet);

        base.OnDestroy();
    }

    public void StartLevel(string levelName)
    {
        if (!SystemReady)
        {
            DebugService.LogError("LevelManagerSystem is not ready");
            return;
        }

        if (SyncedValues.CanWriteValues)
        {
            SyncedValues.SetOrCreate(new SyncedValueCurrentLevel() { Name = levelName });
        }
        else
        {
            DebugService.LogError("Client cannot start a level. The server must do that.");
        }
    }

    private void OnLevelSet(in SyncedValueCurrentLevel newValue)
    {
        if (!newValue.Name.IsNullOrEmpty())
            StartLevelInternal(newValue.Name);
    }

    private void StartLevelInternal(string levelName)
    {
        Level lvl = LevelBank.Levels.Find((x) => x.name == levelName);
        if (lvl)
            StartLevelInternal(lvl);
        else
            Debug.LogError($"Could not start level {levelName}. It was not found in the level bank. " +
                $"The bank is a scriptable object named LevelBank.");
    }

    private void StartLevelInternal(Level level)
    {
        if (IsLevelStarted)
        {
            DebugService.LogError("Cannot start another level (not yet implemented)");
            return;
        }

        SimulationController.Instance.SubmitInput(new SimCommandLoadScene() { SceneName = SimManagersScene.SceneName });
        SimulationController.Instance.SubmitInput(new SimCommandLoadPresentationScene() { SceneName = SimBasePresentationScene.SceneName });

        foreach (SceneInfo scene in level.SimulationScenes)
        {
            SimulationController.Instance.SubmitInput(new SimCommandLoadScene() { SceneName = scene.SceneName });
        }
        foreach (SceneInfo scene in level.PresentationScenes)
        {
            SimulationController.Instance.SubmitInput(new SimCommandLoadPresentationScene() { SceneName = scene.SceneName });
        }
        IsLevelStarted = true;
    }

    public void RemoveLevel(Level level)
    {
        throw new System.NotImplementedException();
    }

}

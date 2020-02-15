using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : GameSystem<LevelManager>
{
    public SceneInfo SimManagersScene;
    public SceneInfo SimBasePresentationScene;
    public LevelBank LevelBank;

    public override bool SystemReady => true;
    public bool IsLevelStarted { get; private set; }

    public override void OnGameStart()
    {
        base.OnGameStart();

        string levelToPlay = null;

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

        if (!levelToPlay.IsNullOrEmpty())
            StartLevel(levelToPlay);
    }

    public void StartLevel(string levelName)
    {
        Level lvl = LevelBank.Levels.Find((x) => x.name == levelName);
        if (lvl)
            StartLevel(lvl);
    }

    public void StartLevel(Level level)
    {
        if (IsLevelStarted)
            throw new System.NotImplementedException();

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

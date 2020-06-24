

using SimulationControl;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngineX;

public static class QuickStart
{
    public static bool HasEverQuickStarted { get; private set; } = false;

    static QuickStartAssets Assets => QuickStartAssets.instance;

    static Coroutine s_currentRoutine;

    public static void Start(QuickStartSettings settings)
    {
        HasEverQuickStarted = true;
        CoreServiceManager.AddInitializationCallback(() =>
        {
            Log.Info("QuickStart: " + settings.ToString());
            StopRoutine();
            s_currentRoutine = CoroutineLauncherService.Instance.StartCoroutine(StartRoutine(settings));
        });
    }

    public static void StartFromScratch(int playerProfileLocalId = 0)
    {
        HasEverQuickStarted = true;
        CoreServiceManager.AddInitializationCallback(() =>
        {
            Log.Info("QuickStart: from scratch - profile:" + playerProfileLocalId);
            StopRoutine();
            PlayerProfileService.Instance.SetPlayerProfile(playerProfileLocalId);
            GameStateManager.TransitionToState(Assets.rootMenu);
        });
    }

    static void StopRoutine()
    {
        if (s_currentRoutine != null)
        {
            SceneService.UnloadAsync(Assets.emptyScene);
            CoroutineLauncherService.Instance.StopCoroutine(s_currentRoutine);
        }
    }

    static IEnumerator StartRoutine(QuickStartSettings settings)
    {
        SimulationWorldSystem.ClearAllSimulationWorlds();

        SceneService.Load(Assets.emptyScene.name, LoadSceneMode.Additive, LocalPhysicsMode.Physics3D);

        yield return null; // wait for scene load

        PlayerProfileService.Instance.SetPlayerProfile(settings.localProfileId);

        switch (settings.playMode)
        {
            case QuickStartSettings.PlayMode.Local:
                yield return StartLocal(settings);
                break;
            case QuickStartSettings.PlayMode.OnlineClient:
                yield return StartClient(settings);
                break;
            case QuickStartSettings.PlayMode.OnlineServer:
                yield return StartServer(settings);
                break;
        }

        SceneService.UnloadAsync(Assets.emptyScene);
    }

    static IEnumerator StartLocal(QuickStartSettings settings)
    {
        LoadingScreenUIController.displayedStatus = "Loading...";

        OnlineService.SetTargetRole(OnlineRole.None);
        while (OnlineService.IsChangingRole
            || OnlineService.CurrentRole != OnlineRole.None)
        {
            yield return null;
        }

        GameStateManager.TransitionToState(Assets.inGameLocal, new GameStateParamLevelName(settings.level));
    }


    static IEnumerator StartClient(QuickStartSettings settings)
    {
        LoadingScreenUIController.displayedStatus = "Waiting for online services...";

        OnlineService.SetTargetRole(OnlineRole.Client);
        while (OnlineService.IsChangingRole
            || OnlineService.CurrentRole != OnlineRole.Client
            || OnlineService.ClientInterface == null)
        {
            yield return null;
        }

        if (string.IsNullOrEmpty(settings.serverName))
        {
            LoadingScreenUIController.displayedStatus = "Loading...";
            GameStateManager.TransitionToState(Assets.lobbyClient);
        }
        else
        {
            LoadingScreenUIController.displayedStatus = "Looking for server [" + settings.serverName + "] ...";

            INetworkInterfaceSession foundSession = null;

            IEnumerator WaitForServerToAppear()
            {
                float elapsedTime = 0;
                while (foundSession == null
                    && (elapsedTime < Assets.searchForSeverTimeout || Assets.searchForSeverTimeout == -1)
                    && OnlineService.ClientInterface != null)
                {
                    foreach (INetworkInterfaceSession session in OnlineService.ClientInterface.AvailableSessions)
                    {
                        if (session.HostName == settings.serverName)
                        {
                            foundSession = session;
                            break;
                        }
                    }
                    elapsedTime += Time.unscaledDeltaTime;
                    yield return null;
                }
            }

            yield return WaitForServerToAppear();

            if (foundSession == null)
            {
                string message = "Failed client quickstart. Could not find server with name [" + settings.serverName + "] in time.";
                DebugScreenMessage.DisplayMessage(message);
                Log.Warning(message);
                GameStateManager.TransitionToState(Assets.rootMenu);
            }
            else
            {
                LoadingScreenUIController.displayedStatus = "Connecting to server [" + settings.serverName + "] ...";

                int success = -1; // -1 -> waiting for result    0 -> failure        1 -> success
                OnlineService.ClientInterface.ConnectToSession(foundSession, (bool r, string message) =>
                {
                    if (r)
                        success = 1;
                    else
                        success = 0;
                });

                // wait for connection result
                while (success == -1)
                {
                    yield return null;
                }

                if (success == 0)
                {
                    string message = "Failed client quickstart. Could not connect to server [" + settings.serverName + "].";
                    DebugScreenMessage.DisplayMessage(message);
                    Log.Error(message);

                    GameStateManager.TransitionToState(Assets.rootMenu);
                }
                else
                {
                    // success!
                    LoadingScreenUIController.displayedStatus = "Loading...";
                    GameStateManager.TransitionToState(Assets.inGameClient);
                }
            }
        }
    }


    static IEnumerator StartServer(QuickStartSettings settings)
    {
        LoadingScreenUIController.displayedStatus = "Waiting for online services...";

        OnlineService.SetTargetRole(OnlineRole.Server);
        while (OnlineService.IsChangingRole
            || OnlineService.CurrentRole != OnlineRole.Server
            || OnlineService.ServerInterface == null)
        {
            yield return null;
        }

        if (string.IsNullOrEmpty(settings.serverName))
        {
            LoadingScreenUIController.displayedStatus = "Loading...";
            GameStateManager.TransitionToState(Assets.lobbyServer);
        }
        else
        {
            // Create session

            int success = -1; // -1 -> waiting for result    0 -> failure        1 -> success

            LoadingScreenUIController.displayedStatus = "Creating session ...";
            OnlineService.ServerInterface.CreateSession(settings.serverName, (bool r, string message) =>
            {
                if (r)
                    success = 1;
                else
                    success = 0;
            });

            // wait for creation result
            while (success == -1)
            {
                yield return null;
            }


            if (success == 0)
            {
                string message = "Failed server quickstart. Could not create session [" + settings.serverName + "].";
                DebugScreenMessage.DisplayMessage(message);
                Log.Error(message);
                GameStateManager.TransitionToState(Assets.rootMenu);
            }
            else
            {
                // success!
                LoadingScreenUIController.displayedStatus = "Loading ...";
                GameStateManager.TransitionToState(Assets.inGameServer, new GameStateParamLevelName(settings.level));
            }
        }
    }
}
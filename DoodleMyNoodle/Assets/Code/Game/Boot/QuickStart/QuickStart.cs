using SimulationControl;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngineX;

public static class QuickStart
{
    public static bool HasEverQuickStarted { get; private set; } = false;
    static QuickStartAssets Assets => QuickStartAssets.Instance;
    static Coroutine s_currentRoutine;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void StaticReset()
    {
        HasEverQuickStarted = false;
        s_currentRoutine = null;
    }

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

    public static void StartFromScratch(int playerProfileLocalId)
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
            CoroutineLauncherService.Instance?.StopCoroutine(s_currentRoutine);
        }
    }

    static IEnumerator StartRoutine(QuickStartSettings settings)
    {
        SimulationWorldSystem.ClearAllSimulationWorlds();

        SceneService.Load(Assets.emptyScene.name, LoadSceneMode.Additive, LocalPhysicsMode.Physics3D);

        yield return null; // wait for scene load

        PlayerProfileService.Instance.SetPlayerProfile(settings.LocalProfileId);

        switch (settings.PlayMode)
        {
            case QuickStartSettings.EPlayMode.Local:
                yield return StartLocal(settings);
                break;
            case QuickStartSettings.EPlayMode.OnlineClient:
                yield return StartClient(settings);
                break;
            case QuickStartSettings.EPlayMode.OnlineServer:
                yield return StartServer(settings);
                break;
        }

        SceneService.UnloadAsync(Assets.emptyScene);
    }

    static IEnumerator StartLocal(QuickStartSettings settings)
    {
        LoadingScreenUIController.DisplayedStatus = "Loading...";

        OnlineService.SetTargetRole(OnlineRole.None);
        while (OnlineService.IsChangingRole
            || OnlineService.CurrentRole != OnlineRole.None)
        {
            yield return null;
        }

        GameStateManager.TransitionToState(Assets.inGameLocal, new GameStateParamMapName(settings.Map));
    }


    static IEnumerator StartClient(QuickStartSettings settings)
    {
        LoadingScreenUIController.DisplayedStatus = "Waiting for online services...";

        OnlineService.SetTargetRole(OnlineRole.Client);
        while (OnlineService.IsChangingRole
            || OnlineService.CurrentRole != OnlineRole.Client
            || OnlineService.ClientInterface == null)
        {
            yield return null;
        }

        if (string.IsNullOrEmpty(settings.ServerName))
        {
            LoadingScreenUIController.DisplayedStatus = "Loading...";
            GameStateManager.TransitionToState(Assets.lobbyClient);
        }
        else
        {
            LoadingScreenUIController.DisplayedStatus = "Looking for server [" + settings.ServerName + "] ...";

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
                        if (session.HostName == settings.ServerName)
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
                string message = "Failed client quickstart. Could not find server with name [" + settings.ServerName + "] in time.";
                DebugScreenMessage.DisplayMessage(message);
                Log.Warning(message);
                GameStateManager.TransitionToState(Assets.rootMenu);
            }
            else
            {
                LoadingScreenUIController.DisplayedStatus = "Connecting to server [" + settings.ServerName + "] ...";

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
                    string message = "Failed client quickstart. Could not connect to server [" + settings.ServerName + "].";
                    DebugScreenMessage.DisplayMessage(message);
                    Log.Error(message);

                    GameStateManager.TransitionToState(Assets.rootMenu);
                }
                else
                {
                    // success!
                    LoadingScreenUIController.DisplayedStatus = "Loading...";
                    GameStateManager.TransitionToState(Assets.inGameClient);
                }
            }
        }
    }


    static IEnumerator StartServer(QuickStartSettings settings)
    {
        LoadingScreenUIController.DisplayedStatus = "Waiting for online services...";

        OnlineService.SetTargetRole(OnlineRole.Server);
        while (OnlineService.IsChangingRole
            || OnlineService.CurrentRole != OnlineRole.Server
            || OnlineService.ServerInterface == null)
        {
            yield return null;
        }

        if (string.IsNullOrEmpty(settings.ServerName))
        {
            LoadingScreenUIController.DisplayedStatus = "Loading...";
            GameStateManager.TransitionToState(Assets.lobbyServer);
        }
        else
        {
            // Create session

            int success = -1; // -1 -> waiting for result    0 -> failure        1 -> success

            LoadingScreenUIController.DisplayedStatus = "Creating session ...";
            OnlineService.ServerInterface.CreateSession(settings.ServerName, (bool r, string message) =>
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
                string message = "Failed server quickstart. Could not create session [" + settings.ServerName + "].";
                DebugScreenMessage.DisplayMessage(message);
                Log.Error(message);
                GameStateManager.TransitionToState(Assets.rootMenu);
            }
            else
            {
                // success!
                LoadingScreenUIController.DisplayedStatus = "Loading ...";
                GameStateManager.TransitionToState(Assets.inGameServer, new GameStateParamMapName(settings.Map));
            }
        }
    }
}
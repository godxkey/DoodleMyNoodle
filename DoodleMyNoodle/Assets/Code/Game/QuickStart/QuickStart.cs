using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class QuickStart
{
    static QuickStartAssets assets => QuickStartAssets.instance;


    public static void Start(QuickStartSettings settings)
    {
        CoreServiceManager.AddInitializationCallback(() =>
        {
            CoroutineLauncherService.Instance.StartCoroutine(StartRoutine(settings));
        });
    }

    static IEnumerator StartRoutine(QuickStartSettings settings)
    {
        DebugService.Log("QuickStart: " + settings.ToString());

        SceneService.Load(assets.emptyScene);

        yield return null;

        if (string.IsNullOrEmpty(settings.playerName) == false)
        {
            PlayerProfileService.Instance.playerName = settings.playerName;
        }

        switch (settings.playMode)
        {
            case QuickStartSettings.PlayMode.None:
                yield return StartNone(settings);
                break;
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
    }

    static IEnumerator StartNone(QuickStartSettings settings)
    {
        GameStateManager.TransitionToState(assets.rootMenu);
        yield return null;
    }

    static IEnumerator StartLocal(QuickStartSettings settings)
    {
        LoadingScreenUIController.displayedStatus = "Loading...";

        OnlineService.SetTargetRole(OnlineRole.None);
        while (OnlineService.isChangingRole
            || OnlineService.currentRole != OnlineRole.None)
        {
            yield return null;
        }

        if (settings.level.IsNullOrEmpty())
        {
            GameStateManager.TransitionToState(assets.lobbyLocal);
        }
        else
        {
            GameStateManager.TransitionToState(assets.inGameLocal, new GameStateParamLevelName(settings.level));
        }
    }


    static IEnumerator StartClient(QuickStartSettings settings)
    {
        LoadingScreenUIController.displayedStatus = "Waiting for online services...";

        OnlineService.SetTargetRole(OnlineRole.Client);
        while (OnlineService.isChangingRole
            || OnlineService.currentRole != OnlineRole.Client
            || OnlineService.clientInterface == null)
        {
            yield return null;
        }

        if (settings.serverName.IsNullOrEmpty())
        {
            LoadingScreenUIController.displayedStatus = "Loading...";
            GameStateManager.TransitionToState(assets.lobbyClient);
        }
        else
        {
            LoadingScreenUIController.displayedStatus = "Looking for server [" + settings.serverName + "] ...";

            INetworkInterfaceSession foundSession = null;

            IEnumerator WaitForServerToAppear()
            {
                float elapsedTime = 0;
                while (foundSession == null && (elapsedTime < assets.searchForSeverTimeout || assets.searchForSeverTimeout == -1))
                {
                    foreach (INetworkInterfaceSession session in OnlineService.clientInterface.availableSessions)
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
                DebugService.LogError("Failed client quickstart. Could not find server with name [" + settings.serverName + "] in time.", true);
                GameStateManager.TransitionToState(assets.rootMenu);
            }
            else
            {
                LoadingScreenUIController.displayedStatus = "Connecting to server [" + settings.serverName + "] ...";

                int success = -1; // -1 -> waiting for result    0 -> failure        1 -> success
                OnlineService.clientInterface.ConnectToSession(foundSession, (bool r, string message) =>
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
                    DebugService.LogError("Failed client quickstart. Could not connect to server [" + settings.serverName + "].", true);
                    GameStateManager.TransitionToState(assets.rootMenu);
                }
                else
                {
                    // success!
                    LoadingScreenUIController.displayedStatus = "Loading...";
                    GameStateManager.TransitionToState(assets.inGameClient);
                }
            }
        }
    }


    static IEnumerator StartServer(QuickStartSettings settings)
    {
        LoadingScreenUIController.displayedStatus = "Waiting for online services...";

        OnlineService.SetTargetRole(OnlineRole.Server);
        while (OnlineService.isChangingRole
            || OnlineService.currentRole != OnlineRole.Server
            || OnlineService.serverInterface == null)
        {
            yield return null;
        }

        if (settings.serverName.IsNullOrEmpty())
        {
            LoadingScreenUIController.displayedStatus = "Loading...";
            GameStateManager.TransitionToState(assets.lobbyServer);
        }
        else
        {
            // Create session

            int success = -1; // -1 -> waiting for result    0 -> failure        1 -> success

            LoadingScreenUIController.displayedStatus = "Creating session ...";
            OnlineService.serverInterface.CreateSession(settings.serverName, (bool r, string message) =>
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
                DebugService.LogError("Failed server quickstart. Could not create session [" + settings.serverName + "].", true);
                GameStateManager.TransitionToState(assets.rootMenu);
            }
            else
            {
                // success!
                LoadingScreenUIController.displayedStatus = "Loading ...";
                GameStateManager.TransitionToState(assets.inGameServer);
            }
        }
    }
}
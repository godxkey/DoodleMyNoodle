using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuOnlineRoleChoice : MonoBehaviour
{
    public void LaunchClient()
    {
        Debug.Log(GameStateManager.currentGameState);
        ((GameStateRootMenu)GameStateManager.currentGameState).JoinLobbyAsClient();
    }

    public void LaunchServer()
    {
        ((GameStateRootMenu)GameStateManager.currentGameState).JoinLobbyAsServer();
    }
}

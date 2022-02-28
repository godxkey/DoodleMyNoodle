using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//#if UNITY_EDITOR
//using UnityEditor;
//#endif

[CreateAssetMenu(menuName = "DoodleMyNoodle/QuickStart Assets")]
public class QuickStartAssets : ScriptableObject
{
    public static QuickStartAssets Instance
    {
        get
        {
            if (s_instance == null)
                s_instance = (QuickStartAssets)Resources.Load("QuickStartAssets");

            return s_instance;
        }
    }
    static QuickStartAssets s_instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void StaticReset()
    {
        s_instance = null;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public SceneInfo emptyScene;

    [Header("GameStates")]
    public GameStateDefinitionInGameLocal inGameLocal;
    public GameStateDefinitionInGameClient inGameClient;
    public GameStateDefinitionInGameServer inGameServer;
    public GameStateDefinitionLobbyLocal lobbyLocal;
    public GameStateDefinitionLobbyServer lobbyServer;
    public GameStateDefinitionLobbyClient lobbyClient;
    public GameStateDefinitionRootMenu rootMenu;

    [Header("Settings")]
    public float searchForSeverTimeout = -1;
}

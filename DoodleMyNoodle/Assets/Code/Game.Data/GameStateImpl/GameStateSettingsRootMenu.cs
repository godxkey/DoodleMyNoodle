using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DoodleMyNoodle/GameState Settings/RootMenu")]
public class GameStateSettingsRootMenu : GameStateSettings
{
    public float onlineRolePickTimeout = 10f;
    public GameStateSettings gameStateIfClient;
    public GameStateSettings gameStateIfServer;
    public GameStateSettings gameStateIfLocal;
}

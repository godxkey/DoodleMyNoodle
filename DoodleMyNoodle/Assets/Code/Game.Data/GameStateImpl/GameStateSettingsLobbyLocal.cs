using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DoodleMyNoodle/GameState Settings/LobbyLocal")]
public class GameStateSettingsLobbyLocal : GameStateSettings
{
    public GameStateSettings gameStateIfReturn;
    public GameStateSettings gameStateIfCreateGame;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DoodleMyNoodle/GameState Definition/LobbyLocal")]
public class GameStateDefinitionLobbyLocal : GameStateDefinition
{
    public GameStateDefinition gameStateIfReturn;
    public GameStateDefinition gameStateIfCreateGame;

    public override void DeclareLinks(System.Action<GameStateDefinition> declare)
    {
        base.DeclareLinks(declare);
        declare(gameStateIfReturn);
        declare(gameStateIfCreateGame);
    }
}

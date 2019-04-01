using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DoodleMyNoodle/GameState Definition/LobbyServer")]
public class GameStateDefinitionLobbyServer : GameStateDefinitionLobbyBase
{
    public GameStateDefinition gameStateIfCreateSession;

    public override void DeclareLinks(System.Action<GameStateDefinition> declare)
    {
        base.DeclareLinks(declare);
        declare(gameStateIfCreateSession);
    }
}

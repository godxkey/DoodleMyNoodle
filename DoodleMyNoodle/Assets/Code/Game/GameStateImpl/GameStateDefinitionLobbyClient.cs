using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DoodleMyNoodle/GameState Definition/LobbyClient")]
public class GameStateDefinitionLobbyClient : GameStateDefinitionLobbyBase
{
    public GameStateDefinition gameStateIfJoinSession;

    public override void DeclareLinks(System.Action<GameStateDefinition> declare)
    {
        base.DeclareLinks(declare);
        declare(gameStateIfJoinSession);
    }
}

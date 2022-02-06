using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DoodleMyNoodle/GameState Definition/RootMenu")]
public class GameStateDefinitionRootMenu : GameStateDefinition
{
    public float onlineRolePickTimeout = 10f;
    public GameStateDefinition gameStateIfClient;
    public GameStateDefinition gameStateIfServer;
    public GameStateDefinition gameStateIfLocal;

    public override void DeclareLinks(System.Action<GameStateDefinition> declare)
    {
        declare(gameStateIfClient);
        declare(gameStateIfServer);
        declare(gameStateIfLocal);
    }
}

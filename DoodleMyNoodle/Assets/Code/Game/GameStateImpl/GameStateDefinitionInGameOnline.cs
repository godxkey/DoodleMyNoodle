using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameStateDefinitionInGameOnline : GameStateDefinitionInGameBase
{
    public GameStateDefinition gameStateIfDisconnect;

    public override void DeclareLinks(System.Action<GameStateDefinition> declare)
    {
        base.DeclareLinks(declare);
        declare(gameStateIfDisconnect);
    }
}

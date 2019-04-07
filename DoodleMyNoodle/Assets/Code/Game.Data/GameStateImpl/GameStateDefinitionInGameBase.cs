using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameStateDefinitionInGameBase : GameStateDefinition
{
    public GameStateDefinition gameStateIfReturn;

    public override void DeclareLinks(System.Action<GameStateDefinition> declare)
    {
        base.DeclareLinks(declare);
        declare(gameStateIfReturn);
    }
}

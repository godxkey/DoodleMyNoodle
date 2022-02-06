
using System;
using System.Collections.Generic;

// A runtime representation of a gameState in a "gameState network"
public class GameStateNode
{
    public readonly int index;
    public readonly GameStateDefinition gameStateDefinition;
    public List<GameStateNode> neighbors = new List<GameStateNode>(8);

    public GameStateNode(GameStateDefinition gameStateDefinition, int index)
    {
        this.gameStateDefinition = gameStateDefinition;
        this.index = index;
    }

    public void PopulateNeighbors(Func<GameStateDefinition, GameStateNode> getNeighborInstance)
    {
        System.Action<GameStateDefinition> declarationMethod = (definition) =>
        {
            if(definition != null)
            {
                GameStateNode node = getNeighborInstance(definition);
                if(node != null)
                    neighbors.Add(node);
            }
        };
        gameStateDefinition.DeclareLinks(declarationMethod);
    }
}

using System;
using System.Collections.Generic;

public class GameStateDefinitionGraph
{
    public GameStateNode root { get; private set; }

    public GameStateDefinition[] gameStateDefinitions { get; private set; }
    public Dictionary<GameStateDefinition, GameStateNode> nodeDictionary { get; private set; } = new Dictionary<GameStateDefinition, GameStateNode>();
    public GameStateNode[] nodeArray { get; private set; }


    // The first definition should be the root
    public GameStateDefinitionGraph(GameStateDefinition[] gameStateDefinitions)
    {
        this.gameStateDefinitions = gameStateDefinitions;

        nodeArray = new GameStateNode[gameStateDefinitions.Length];

        for (int i = 0; i < gameStateDefinitions.Length; i++)
        {
            GameStateDefinition definition = gameStateDefinitions[i];
            if (definition == null)
                continue;

            GameStateNode newNode = new GameStateNode(definition, i);

            if (root == null)
            {
                root = newNode; // only first node will apply this
            }

            nodeArray[i] = newNode;
            nodeDictionary.Add(definition, newNode);
        }


        foreach (var node in nodeArray)
        {
            node.PopulateNeighbors(FindGameStateNode);
        }
    }

    public GameStateNode FindGameStateNode(GameStateDefinition definition)
    {
        if (definition == null)
        {
            return null;
        }

        return nodeDictionary[definition];
    }


    public List<GameStateDefinition> FindPathToGameState(GameStateDefinition current, GameStateDefinition destination)
    {
        if (current == null || destination == null)
        {
            throw new NullReferenceException("Trying to find path to game state, but the method was called with null arguments");
        }

        // ___________________________________________ Variable setup ___________________________________________ //
        GameStateNode startNode = nodeDictionary[destination];
        int[] distances = new int[nodeArray.Length];
        GameStateNode[] previous = new GameStateNode[nodeArray.Length];
        List<GameStateNode> queue = new List<GameStateNode>(nodeArray);

        for (int i = 0; i < distances.Length; i++)
        {
            distances[i] = int.MaxValue - 1;
        }

        distances[startNode.index] = 0;

        // ___________________________________________ Helper methods ___________________________________________ //
        int DistanceTo(GameStateNode node)
        {
            return distances[node.index];
        }
        void SetDistanceTo(GameStateNode node, int dist)
        {
            distances[node.index] = dist;
        }
        void SetPrevTo(GameStateNode node, GameStateNode prev)
        {
            previous[node.index] = prev;
        }

        GameStateNode PopNodeWithLowestDist()
        {
            int record = int.MaxValue;
            GameStateNode recordHolder = null;
            int recordI = -1;
            for (int i = 0; i < queue.Count; i++)
            {
                if (DistanceTo(queue[i]) < record)
                {
                    record = DistanceTo(queue[i]);
                    recordHolder = queue[i];
                    recordI = i;
                }
            }

            queue.RemoveAt(recordI);

            return recordHolder;
        }

        // ___________________________________________ Algo ___________________________________________ //
        while (queue.Count > 0)
        {
            GameStateNode u = PopNodeWithLowestDist();

            int fullPathLengthToNeighborsOfU = DistanceTo(u) + 1;
            for (int i = 0; i < u.neighbors.Count; i++)
            {
                if (fullPathLengthToNeighborsOfU < DistanceTo(u.neighbors[i]))
                {
                    SetDistanceTo(u.neighbors[i], fullPathLengthToNeighborsOfU);
                    SetPrevTo(u.neighbors[i], u);
                }
            }
        }

        // ___________________________________________ Build path ___________________________________________ //

        GameStateNode toAdd = nodeDictionary[current];
        List<GameStateDefinition> path = new List<GameStateDefinition>();

        while (toAdd != null)
        {
            path.Add(toAdd.gameStateDefinition);
            toAdd = previous[toAdd.index];
        }

        return path;
    }


}
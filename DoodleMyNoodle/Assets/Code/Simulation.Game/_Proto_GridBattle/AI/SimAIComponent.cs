using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimAIComponent : SimComponent, ISimTickable
{
    [System.Serializable]
    struct SerializedData
    {
        public bool TurnPlayed;
    }

    void ISimTickable.OnSimTick()
    {
        if (SimTurnManager.Instance.IsMyTurn(_team.Team))
        {
            if (!_data.TurnPlayed)
            {
                _data.TurnPlayed = true;

                Fix64 randomDecision = Simulation.Random.Range(0,3);

                Debug.Log($"{SimObjectId} picking direction on tick " + Simulation.TickId);
                if (randomDecision < 1)
                {
                    _gridWalker.TryWalkTo(_gridWalker.TileId + Vector2Int.right);
                }
                else if (randomDecision < 2)
                {
                    _gridWalker.TryWalkTo(_gridWalker.TileId + Vector2Int.left);
                }
                else
                {
                    _gridWalker.TryWalkTo(_gridWalker.TileId + Vector2Int.down);
                }
            }
        }
        else
        {
            _data.TurnPlayed = false;
        }
    }

    #region Serialized Data Methods
    [UnityEngine.SerializeField]
    [AlwaysExpand]
    SerializedData _data = new SerializedData()
    {
        TurnPlayed = false
    };

    public override void SerializeToDataStack(SimComponentDataStack dataStack)
    {
        base.SerializeToDataStack(dataStack);
        dataStack.Push(_data);
    }

    public override void DeserializeFromDataStack(SimComponentDataStack dataStack)
    {
        _data = (SerializedData)dataStack.Pop();
        base.DeserializeFromDataStack(dataStack);
    }
    #endregion

    #region Component Caching
    [System.NonSerialized] SimGridWalkerComponent _gridWalker;
    [System.NonSerialized] SimTeamMemberComponent _team;
    public override void OnAddedToRuntime()
    {
        base.OnAddedToRuntime();
        _gridWalker = GetComponent<SimGridWalkerComponent>();
        _team = GetComponent<SimTeamMemberComponent>();
    }
    #endregion
}

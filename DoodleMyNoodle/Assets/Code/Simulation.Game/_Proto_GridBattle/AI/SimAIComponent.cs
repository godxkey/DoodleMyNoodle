using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SimPawnControllerComponent))]
[RequireComponent(typeof(SimTeamMemberComponent))]
public class SimAIComponent : SimComponent, 
    ISimTickable,
    ISimTargetPawnChangeListener
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

                if (_pawnGridWalker)
                {
                    Fix64 randomDecision = Simulation.Random.Range(0, 3);

                    if (randomDecision < 1)
                    {
                        _pawnGridWalker.TryWalkTo(_pawnGridWalker.TileId + Vector2Int.right);
                    }
                    else if (randomDecision < 2)
                    {
                        _pawnGridWalker.TryWalkTo(_pawnGridWalker.TileId + Vector2Int.left);
                    }
                    else
                    {
                        _pawnGridWalker.TryWalkTo(_pawnGridWalker.TileId + Vector2Int.down);
                    }
                }
            }
        }
        else
        {
            _data.TurnPlayed = false;
        }
    }

    void ISimTargetPawnChangeListener.OnTargetPawnChanged()
    {
        UpdateCachedPawnComponents();
    }

    #region Pawn Component Caching
    [System.NonSerialized] SimGridWalkerComponent _pawnGridWalker;
    void UpdateCachedPawnComponents()
    {
        if(_targetPawn.TargetPawn)
        {
            _pawnGridWalker = _targetPawn.TargetPawn.GetComponent<SimGridWalkerComponent>();
        }
    }
    #endregion

    #region Component Caching
    [System.NonSerialized] SimTeamMemberComponent _team;
    [System.NonSerialized] SimPawnControllerComponent _targetPawn;
    public override void OnAddedToRuntime()
    {
        base.OnAddedToRuntime();
        _team = GetComponent<SimTeamMemberComponent>();
        _targetPawn = GetComponent<SimPawnControllerComponent>();
        
        UpdateCachedPawnComponents();
    }
    #endregion

    #region Serialized Data Methods
    [UnityEngine.SerializeField]
    [CCC.InspectorDisplay.AlwaysExpand]
    SerializedData _data = new SerializedData()
    {
        // define default values here
    };

    public override void PushToDataStack(SimComponentDataStack dataStack)
    {
        base.PushToDataStack(dataStack);
        dataStack.Push(_data);
    }

    public override void PopFromDataStack(SimComponentDataStack dataStack)
    {
        _data = (SerializedData)dataStack.Pop();
        base.PopFromDataStack(dataStack);
    }
    #endregion
}

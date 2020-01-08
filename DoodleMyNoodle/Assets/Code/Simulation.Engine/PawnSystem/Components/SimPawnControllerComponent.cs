using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An entity with this component can control a pawn
/// </summary>
public class SimPawnControllerComponent : SimComponent
{
    // fbessette: This could be changed to a List<SimPawnComponent> if we want the player to be able to 
    //            control multiple pawns AT THE SAME TIME (like an RTS with multiselection)
    [System.Serializable]
    struct SerializedData
    {
        [ReadOnlyInEditMode]
        public SimPawnComponent TargetPawn;
    }

    // for now, no need to be saved. it should be a permanent setting
    [SerializeField] bool _destroySelfIfNoTarget = true;

    public bool DestroySelfIfNoTarget => _destroySelfIfNoTarget;
    public SimPawnComponent TargetPawn { get => _data.TargetPawn; set => _data.TargetPawn = value; }

    #region Serialized Data Methods
    [UnityEngine.SerializeField]
    [AlwaysExpand]
    SerializedData _data = new SerializedData()
    {
        // define default values here
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
}

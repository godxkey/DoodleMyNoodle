using CCC.InspectorDisplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX.InspectorDisplay;

public class SimDamageOnContactComponent : SimComponent, ISimTickable
{
    [System.Serializable]
    struct SerializedData
    {
        public int Damage;
        public bool DestroyOnContact;
    }

    public void OnSimTick()
    {
        SimTileId_OLD myTileId = SimTransform.GetTileId();

        foreach (SimPawnComponent pawn in Simulation.EntitiesWithComponent<SimPawnComponent>())
        {
            // are we on the same tile as the pawn ?
            if (pawn.SimTransform.GetTileId() == myTileId && pawn.SimEntity != SimEntity)
            {
                // damage pawn if possible
                if(pawn.TryGetComponent(out SimHealthStatComponent pawnHealth))
                {
                    pawnHealth.IncreaseValue(-_data.Damage);
                }

                // destroy self
                Simulation.Destroy(SimEntity);

                break;
            }
        }
    }

    #region Serialized Data Methods
    [UnityEngine.SerializeField]
    [AlwaysExpand]
    SerializedData _data = new SerializedData()
    {
        // define default values here
        DestroyOnContact = true
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

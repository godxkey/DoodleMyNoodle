using CCC.InspectorDisplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimCharacterAttackComponent : SimEventComponent
{
    [System.Serializable]
    struct SerializedData
    {
        public int AttackDamage;
    }

    public bool WantsToAttack = false;
    public bool ChoiceMade = false;

    public int AttackDamage { get => _data.AttackDamage; set => _data.AttackDamage = value; }

    public bool TryToAttack(SimTileId simTileId)
    {
        SimEntity simEntity = SimTileHelpers.GetPawnOnTile(simTileId);
        if(simEntity != null)
        {
            SimHealthStatComponent Health = simEntity.GetComponent<SimHealthStatComponent>();
            SimTeamMemberComponent myTeamComponent = GetComponent<SimTeamMemberComponent>();
            SimTeamMemberComponent targetTeamComponent = simEntity.GetComponent<SimTeamMemberComponent>();
            if (Health != null && myTeamComponent != null && targetTeamComponent != null)
            {
                if(myTeamComponent.Team != targetTeamComponent.Team)
                {
                    Health.DecreaseValue(AttackDamage);
                    return true;
                }
            }
        }

        return false;
    }

    #region Serialized Data Methods
    [UnityEngine.SerializeField]
    [AlwaysExpand]
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

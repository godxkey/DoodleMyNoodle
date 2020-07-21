using static fixMath;
using Unity.Entities;
using Unity.Collections;
using UnityEngineX;

public class GameActionInteract : GameAction
{
    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return new UseContract();
    }

    public override bool IsContextValid(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return true;
    }

    public override void Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters)
    {
        accessor.SetOrAddComponentData(context.Entity, new Interacted() { Value = true });
    }
}
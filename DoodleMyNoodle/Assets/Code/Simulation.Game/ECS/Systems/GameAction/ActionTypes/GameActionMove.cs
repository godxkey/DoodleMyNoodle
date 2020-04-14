using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public class GameActionJump : GameAction
{
    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, Entity instigator)
    {
        throw new System.NotImplementedException();
    }

    public override bool IsInstigatorValid(ISimWorldReadAccessor accessor, Entity instigator)
    {
        throw new System.NotImplementedException();
    }

    public override void Use(ISimWorldReadWriteAccessor accessor, Entity instigator, UseData useData)
    {
        throw new System.NotImplementedException();
    }
}
public class GameActionShoot : GameAction
{
    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, Entity instigator)
    {
        throw new System.NotImplementedException();
    }

    public override bool IsInstigatorValid(ISimWorldReadAccessor accessor, Entity instigator)
    {
        throw new System.NotImplementedException();
    }

    public override void Use(ISimWorldReadWriteAccessor accessor, Entity instigator, UseData useData)
    {
        throw new System.NotImplementedException();
    }
}

public class GameActionMove : GameAction
{
    public override bool IsInstigatorValid(ISimWorldReadAccessor accessor, Entity instigator)
    {
        return accessor.HasComponent<ActionPoints>(instigator)
            && accessor.HasComponent<FixTranslation>(instigator);
    }

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, Entity instigator)
    {
        UseContract useContract = new UseContract();
        useContract.ParameterTypes = new ParameterDescription[]
        {
            new GameActionParameterTile.Description()
            {
                IsOptional = false,
                RangeFromInstigator = accessor.GetComponentData<ActionPoints>(instigator).Value,
                Filter = TileFilterFlags.Navigable | TileFilterFlags.Inoccupied
            }
        };

        return useContract;
    }

    public override void Use(ISimWorldReadWriteAccessor accessor, Entity instigator, UseData useData)
    {
        if(useData.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {
            int instigatorAP = accessor.GetComponentData<ActionPoints>(instigator).Value;
            fix3 instigatorPos = accessor.GetComponentData<FixTranslation>(instigator).Value;
            int2 instigatorTile = roundToInt(instigatorPos).xy;

            int costToMove = lengthmanhattan(paramTile.Tile - instigatorTile);

            if(costToMove > instigatorAP)
            {
                return;
            }

            // reduce instigator AP
            CommonWrites.SetStatInt(accessor, instigator, new ActionPoints() { Value = instigatorAP - costToMove });

            // set destination
            accessor.SetOrAddComponentData(instigator, new Destination() { Value = fix3(paramTile.Tile, 0) });
        }
    }
}
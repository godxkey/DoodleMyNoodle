using System;
using static Unity.Mathematics.math;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

public struct TileFinder
{
    public struct Context
    {
        public int2 PawnTile;
        public Entity PawnEntity;
    }

    private ISimWorldReadAccessor _accessor;

    public TileFinder(ISimWorldReadAccessor accessor)
    {
        _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
    }

    public void Find(GameActionParameterTile.Description description, Context context, NativeList<int2> result)
    {
        int2 tileMin = context.PawnTile - int2(description.RangeFromInstigator);
        int2 tileMax = context.PawnTile + int2(description.RangeFromInstigator);

        for (int x = tileMin.x; x <= tileMax.x; x++)
        {
            for (int y = tileMin.y; y <= tileMax.y; y++)
            {
                TestTile(int2(x, y), description, context, result);
            }
        }
    }

    private void TestTile(int2 tilePosition, GameActionParameterTile.Description description, in Context parameters, NativeList<int2> result)
    {
        if (IsTileOk(tilePosition, description, parameters))
        {
            result.Add(tilePosition);
        }
    }

    private bool IsTileOk(int2 tilePosition, GameActionParameterTile.Description description, in Context parameters)
    {
        if (!description.IncludeSelf && parameters.PawnTile.Equals(tilePosition))
        {
            return false;
        }

        // tile entity valid
        Entity tileEntity = CommonReads.GetTileEntity(_accessor, tilePosition);
        if (tileEntity == Entity.Null)
        {
            return false; // invalid entity
        }

        // tile filters
        var tileFlags = _accessor.GetComponent<TileFlagComponent>(tileEntity);
        if ((tileFlags & description.TileFilter) == 0)
        {
            return false; // tile is filtered out
        }

        // Custom tile predicate
        if (description.CustomTilePredicate != null)
        {
            if (!description.CustomTilePredicate(tilePosition, tileEntity, _accessor))
            {
                return false;
            }
        }

        // tile reachable
        if (description.MustBeReachable)
        {
            NativeList<int2> highlightNavigablePath = new NativeList<int2>(Allocator.Temp);
            if (!Pathfinding.FindNavigablePath(_accessor, parameters.PawnTile, tilePosition, description.RangeFromInstigator, highlightNavigablePath))
            {
                return false; // tile is non-reachable
            }
        }

        return true;
    }
}
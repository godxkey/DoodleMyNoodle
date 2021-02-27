using Unity.Mathematics;
using Unity.Entities;
using static fixMath;
using Unity.Collections;
using CCC.Fix2D;

[UpdateInGroup(typeof(MovementSystemGroup))]
public class CreatePathToDestinationSystem : SimSystemBase
{
    NativeList<int2> _tilePath;

    protected override void OnCreate()
    {
        base.OnCreate();

        _tilePath = new NativeList<int2>(Allocator.Persistent);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _tilePath.Dispose();
    }

    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, in FixTranslation pos, in Destination destination) =>
        {
            int2 from = Helpers.GetTile(pos);
            int2 to = Helpers.GetTile(destination.Value);

            bool pathFound = Pathfinding.FindNavigablePath(Accessor, from, to, Pathfinding.MAX_PATH_LENGTH, _tilePath);

            if (pathFound)
            {
                DynamicBuffer<PathPosition> pathBuffer = EntityManager.GetOrAddBuffer<PathPosition>(entity);
                pathBuffer.Clear();

                for (int i = 1; i < _tilePath.Length - 1; i++) // exclude last point since we add 'destination' last
                {
                    pathBuffer.Add(new PathPosition() { Position = Helpers.GetTileCenter(_tilePath[i]) });
                }

                pathBuffer.Add(destination.Value);
            }

            EntityManager.RemoveComponent<Destination>(entity);
        })
            .WithStructuralChanges()
            .WithoutBurst()
            .Run();
    }
}
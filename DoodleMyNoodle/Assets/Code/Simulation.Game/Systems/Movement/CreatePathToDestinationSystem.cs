using Unity.Mathematics;
using Unity.Entities;
using static fixMath;
using Unity.Collections;
using UnityEditor;

[UpdateInGroup(typeof(MovementSystemGroup))]
public class CreatePathToDestinationSystem : SimComponentSystem
{
    NativeList<int2> _pathArray;

    protected override void OnCreate()
    {
        base.OnCreate();

        _pathArray = new NativeList<int2>(Allocator.Persistent);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _pathArray.Dispose();
    }

    protected override void OnUpdate()
    {
        Entities
            .ForEach((Entity entity, ref FixTranslation pos, ref Destination destination) =>
        {
            int2 from = Helpers.GetTile(pos);
            int2 to = Helpers.GetTile(destination.Value);

            bool pathFound = Pathfinding.FindNavigablePath(Accessor, from, to, Pathfinding.MAX_PATH_LENGTH, _pathArray);

            if (pathFound)
            {
                DynamicBuffer<PathPosition> pathBuffer = EntityManager.GetOrAddBuffer<PathPosition>(entity);
                pathBuffer.Clear();

                for (int i = 0; i < _pathArray.Length; i++)
                {
                    pathBuffer.Add(new PathPosition() { Position = Helpers.GetTileCenter(_pathArray[i]) });
                }
            }

            EntityManager.RemoveComponent<Destination>(entity);
        });
    }
}
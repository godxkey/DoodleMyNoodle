using Unity.Mathematics;
using Unity.Entities;
using static fixMath;
using Unity.Collections;
using UnityEditor;

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
        Entities.
            WithNone<PathPosition>()
            .ForEach((Entity entity, ref FixTranslation pos, ref Destination destination) =>
        {
            int2 from = roundToInt(pos.Value).xy;
            int2 to = roundToInt(destination.Value).xy;

            bool pathFound = CommonReads.FindNavigablePath(Accessor, from, to, _pathArray);

            if (pathFound)
            {
                var pathBuffer = EntityManager.AddBuffer<PathPosition>(entity);

                for (int i = 0; i < _pathArray.Length; i++)
                {
                    pathBuffer.Add(new PathPosition() { Position = fix3(_pathArray[i], 0) });
                }
            }
        });
    }
}
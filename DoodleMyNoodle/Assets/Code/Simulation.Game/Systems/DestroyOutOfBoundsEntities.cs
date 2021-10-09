//using CCC.Fix2D;
//using System.Collections.Generic;
//using Unity.Entities;
//using Unity.Mathematics;
//using static fixMath;
//using static Unity.Mathematics.math;

//public class DestroyOutOfBoundsEntities : SimSystemBase
//{
//    private List<Entity> _toDestroy = new List<Entity>();

//    protected override void OnUpdate()
//    {
//        if (HasSingleton<GridInfo>())
//        {
//            Entity gridInfoEntity = GetSingletonEntity<GridInfo>();
//            GridInfo gridInfo = EntityManager.GetComponentData<GridInfo>(gridInfoEntity);

//            Entities
//                .WithoutBurst()
//                .WithStructuralChanges()
//                .ForEach((Entity entity, ref FixTranslation translation) =>
//                {
//                    bool shouldDestroy = false;

//                    if (translation.Value.x < gridInfo.TileMin.x || translation.Value.y < gridInfo.TileMin.y)
//                    {
//                        shouldDestroy = true;
//                    }

//                    if (translation.Value.x > gridInfo.TileMax.x || translation.Value.y > gridInfo.TileMax.y)
//                    {
//                        shouldDestroy = true;
//                    }

//                    if (shouldDestroy)
//                    {
//                        _toDestroy.Add(entity);
//                    }
//                }).Run();

//            foreach (var entity in _toDestroy)
//            {
//                EntityManager.DestroyEntity(entity);
//            }

//            _toDestroy.Clear();
//        }
//    }
//}
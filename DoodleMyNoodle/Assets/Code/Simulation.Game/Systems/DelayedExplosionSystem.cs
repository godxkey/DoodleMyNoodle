using CCC.Fix2D;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public class DelayedExplosionSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ref DelayedExplosion delayedExplosion) =>
        {
            bool readyToExplode = false;

            if (delayedExplosion.UseTime)
            {
                delayedExplosion.TimeDuration -= Time.DeltaTime;

                readyToExplode = delayedExplosion.TurnDuration <= 0;
            }
            else
            {
                if (HasSingleton<NewTurnEventData>())
                {
                    delayedExplosion.TurnDuration--;

                    readyToExplode = delayedExplosion.TurnDuration <= 0;
                }
            }

            if (readyToExplode)
            {
                int2 entityTilePos = Helpers.GetTile(EntityManager.GetComponentData<FixTranslation>(entity));
                CommonWrites.RequestExplosionOnTiles(Accessor, entity, entityTilePos, delayedExplosion.Range, delayedExplosion.Damage);
                PostUpdateCommands.DestroyEntity(entity);
            }
        });
    }
}
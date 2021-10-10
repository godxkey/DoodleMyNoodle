using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public class DestroyAfterDelaySystem : SimSystemBase
{
    private List<Entity> _toDestroy = new List<Entity>();

    protected override void OnUpdate()
    {
        Entities
           .WithoutBurst()
           .WithStructuralChanges()
           .ForEach((Entity entity, ref DestroyAfterDelay destroyAfterDelay) =>
           {
               if (!(destroyAfterDelay.Delay.Type == TimeValue.ValueType.Seconds))
               {
                   if (HasSingleton<NewTurnEventData>())
                   {
                       if (destroyAfterDelay.Delay.Value > 0 && destroyAfterDelay.Delay.Type == TimeValue.ValueType.Rounds && GetSingleton<TurnCurrentTeamSingletonComponent>().Value == 0)
                       {
                           destroyAfterDelay.TrackedTime.Value++;

                           if (destroyAfterDelay.TrackedTime.Value >= destroyAfterDelay.Delay.Value)
                           {
                               _toDestroy.Add(entity);
                           }
                       }
                       else if (destroyAfterDelay.Delay.Value > 0 && destroyAfterDelay.Delay.Type == TimeValue.ValueType.Turns)
                       {
                           destroyAfterDelay.TrackedTime.Value++;

                           if (destroyAfterDelay.TrackedTime.Value >= destroyAfterDelay.Delay.Value)
                           {
                               _toDestroy.Add(entity);
                           }
                       }
                   }
               }
               else if (destroyAfterDelay.Delay.Value > 0 && (Time.ElapsedTime - destroyAfterDelay.TrackedTime.Value) >= destroyAfterDelay.Delay.Value)
               {
                   _toDestroy.Add(entity);
               }
           }).Run();

        foreach (var entity in _toDestroy)
        {
            EntityManager.DestroyEntity(entity);
        }

        _toDestroy.Clear();
    }
}
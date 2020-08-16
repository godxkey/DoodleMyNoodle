using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

//public class TimerCountdownSystem : SimComponentSystem
//{
//    protected override void OnUpdate()
//    {
//        Entities.ForEach((Entity timerEntity, ref Timer timer) =>
//        {
//            if (timer.CanCountdown)
//            {
//                Accessor.SetComponentData(timerEntity, new Timer() { Duration = timer.Duration - Time.DeltaTime, CanCountdown = true });
//            }
//        });
//    }
//}
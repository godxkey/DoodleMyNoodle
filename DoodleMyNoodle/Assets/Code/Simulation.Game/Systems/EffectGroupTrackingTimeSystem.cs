using Unity.Entities;

public class EffectGroupTrackingTimeSystem : SimSystemBase
{
    protected override void OnUpdate()
    {
        DynamicBuffer<EffectGroupBufferSingleton> effectGroupBufferSingleton = GetSingletonBuffer<EffectGroupBufferSingleton>();
        for (int i = effectGroupBufferSingleton.Length - 1; i >= 0; i--)
        {
            EffectGroupBufferSingleton effectGroupBuffer = effectGroupBufferSingleton[i];
            if (Time.ElapsedTime >= effectGroupBuffer.TimeStamp + effectGroupBuffer.Delay)
            {
                effectGroupBufferSingleton.RemoveAt(i);
            }
        }
    }
}